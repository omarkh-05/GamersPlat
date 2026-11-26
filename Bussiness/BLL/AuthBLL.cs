using Azure.Core;
using Bussiness.Helpers;
using Bussiness.Interfaces;
using Data;
using Data.DLL;
using Data.EF;
using Domain.DTOs.Auth;
using Domain.DTOs.User;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Bussiness.BLL
{
    public class AuthBLL : IAuthService
    {
        private readonly AuthDLL _authDLL;
        private readonly IUser _user;
        private readonly AuthHelpers _authHelpers;
        private readonly RefreshTokenBLL _refreshTokenBLL;
        private readonly IRoles _roles;
        private readonly IUserRole _userRole;
        private readonly PasswordResetTokenBLL _passwordResetTokenBLL;
        private readonly IAuditLogService _auditLogService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        readonly GamersPlatDbContext _db;
        public AuthBLL(IUser user, AuthHelpers authHelpers, RefreshTokenBLL refreshTokenBLL, IRoles roles, IUserRole userRole, PasswordResetTokenBLL passwordResetTokenBLL, AuthDLL authDLL, GamersPlatDbContext db, IAuditLogService auditLogService, IHttpContextAccessor httpContextAccessor)
        {
            _authHelpers = authHelpers;
            _refreshTokenBLL = refreshTokenBLL;
            _user = user;
            _roles = roles;
            _userRole = userRole;
            _passwordResetTokenBLL = passwordResetTokenBLL;
            _authDLL = authDLL;
            _db = db;
            _auditLogService = auditLogService;
            _httpContextAccessor = httpContextAccessor;
        }
        public record ServiceResult(bool Success,string Message, int StatusCode);
        // ================ User Auth Management ================
        public Task<int> RegisterWithRoleAsync(UserRegisterRequest request, string roleName)
        {
            if (string.IsNullOrWhiteSpace(request.FullName) ||
                string.IsNullOrWhiteSpace(request.PhoneNumber) ||
                string.IsNullOrWhiteSpace(request.Password))
                throw new Exception("Invalid data");


            if (!IsPasswordStrong(request.Password))
                throw new Exception("Password must be at least 8 characters and contain letters, numbers, and special characters.");

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            return RegisterWithRoleAsync(request, roleName, passwordHash);
        }
        public async Task<int> RegisterWithRoleAsync(UserRegisterRequest request, string roleName, string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(request.FullName) ||
                string.IsNullOrWhiteSpace(request.PhoneNumber) ||
                string.IsNullOrWhiteSpace(request.Password) ||
                string.IsNullOrWhiteSpace(passwordHash))
                throw new Exception("Invalid data");

            var transaction = _db.Database.CurrentTransaction;
            var ownsTransaction = transaction == null;
            if (ownsTransaction)
                transaction = await _db.Database.BeginTransactionAsync();

            try {

            if (await _user.ExistsByPhone(request.PhoneNumber))
                throw new Exception("Phone number already exists");

            var role = await _roles.GetByName(roleName.ToLower());
            if (role == null)
                throw new Exception("Invalid role");

                var user = new User
                {
                    FullName = request.FullName,
                    PhoneNumber = request.PhoneNumber,
                    Email = request.Email,
                    PasswordHash = passwordHash,
                    IsActive = true,
                    CityId = request.CityId
                };

                if (!await _user.Add(user))
                    throw new Exception("Failed to create user");

                var userRole = new UserRole
                {
                    UserId = user.UserId,
                    RoleId = role.RoleId,
                };

                if(!await _userRole.Add(userRole))
                throw new Exception("Failed to add user role");
            if (ownsTransaction)
                await transaction!.CommitAsync();
            var currentActorId = GetCurrentUserId();
            var actorId = currentActorId ?? user.UserId;
            await _auditLogService.LogAsync(actorId, currentActorId.HasValue ? "UserCreated" : "UserRegistered", "User", user.UserId, null, new
            {
                user.FullName,
                user.PhoneNumber,
                user.Email,
                user.CityId,
                Role = roleName
            });
            return user.UserId;
            }
            catch
            {
                if (ownsTransaction && transaction != null)
                    await transaction.RollbackAsync();
                throw new Exception("Failed Register User With Role");
            }
            finally
            {
                if (ownsTransaction && transaction != null)
                    await transaction.DisposeAsync();
            }
        }
        public async Task<TokenResponse> LoginAsync(LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.PhoneNumber) ||
                string.IsNullOrWhiteSpace(request.Password))
                throw new Exception("Invalid data");

            var user = await _authDLL.GetUserAuth(request.PhoneNumber);

            if (user == null || !user.IsActive)
            {
                await _auditLogService.LogAsync(user?.UserId, "LoginFailed", "User", user?.UserId ?? 0, null, new
                {
                    Outcome = "Failed",
                    Reason = user == null ? "UnknownAccount" : "InactiveAccount"
                });
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                await _auditLogService.LogAsync(user.UserId, "LoginFailed", "User", user.UserId, null, new
                {
                    Outcome = "Failed",
                    Reason = "InvalidCredentials"
                });
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            var accessToken = _authHelpers.GenerateAccessToken(user);
            var refreshToken = _authHelpers.GenerateRefreshToken();

            var rt = new RefreshToken
            {
                UserId = user.UserId,
                TokenHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken))),
                ExpiresAt = DateTime.UtcNow.AddDays(31),
                CreatedAt = DateTime.UtcNow,
                IsRevoked = false
            };

            if (!await _refreshTokenBLL.Add(rt))
                throw new Exception("Failed to create refresh token");

            await _auditLogService.LogAsync(user.UserId, "LoginSucceeded", "User", user.UserId, null, new { Outcome = "Succeeded" });

            return new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                UserName = user.FullName ?? "Not User",
                UserRole = user.Role,
            };
        }
        public async Task<bool> LogoutAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                return false;

            var existingToken = await _refreshTokenBLL.GetByToken(refreshToken);

            if (existingToken == null)
                return false;

            if (existingToken.IsRevoked)
            {
                await _auditLogService.LogAsync(existingToken.UserId, "Logout", "User", existingToken.UserId, null, new { Outcome = "Succeeded" });
                return true;
            }

            existingToken.IsRevoked = true;
            existingToken.RevokedAt = DateTime.UtcNow;

            var loggedOut = await _refreshTokenBLL.Update(existingToken);
            if (loggedOut)
                await _auditLogService.LogAsync(existingToken.UserId, "Logout", "User", existingToken.UserId, new { RefreshTokenRevoked = false }, new { RefreshTokenRevoked = true });
            return loggedOut;
        }
        // احتاج اضافة ترانس اكشن عند تعديل التوكن ثم اضافة توكن جديدة للتأكد من عدم تحديث التوكن دون اضافتها في الجدول
        public async Task<TokenResponse> RefreshAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                throw new UnauthorizedAccessException("Invalid refresh request");

            var existingToken = await _refreshTokenBLL.GetByToken(refreshToken);

            if (existingToken == null)
                throw new UnauthorizedAccessException("Invalid refresh request");

            if (existingToken.IsRevoked || existingToken.RevokedAt != null)
                throw new UnauthorizedAccessException("Refresh token is revoked");

            if (existingToken.ExpiresAt <= DateTime.UtcNow)
                throw new UnauthorizedAccessException("Refresh token expired");

            var user = await _authDLL.GetUserAuth(userId: existingToken.UserId);

            if (user == null || !user.IsActive)
                throw new UnauthorizedAccessException("Invalid refresh request");

            var newAccessToken = _authHelpers.GenerateAccessToken(user);
            var newRefreshToken = _authHelpers.GenerateRefreshToken();
            var rotationTime = DateTime.UtcNow;

            var rt = new RefreshToken
            {
                UserId = user.UserId,
                TokenHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(newRefreshToken))),
                ExpiresAt = rotationTime.AddDays(31),
                CreatedAt = rotationTime,
                IsRevoked = false
            };

            var tokenHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken)));
            var rotated = await _refreshTokenBLL.RotateAsync(existingToken.TokenId, tokenHash, rotationTime, rt);
            if (!rotated)
                throw new UnauthorizedAccessException("Refresh token is revoked");

            return new TokenResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                UserName = user.FullName??"",
                UserRole = user.Role,
            };
        }
        public async Task<DTO_HomeCurrnetUserInfo?> CheckAuth(string phoneNumber)
        {
            var user = await _user.GetUserInfoByPhone(phoneNumber);
            var currentUserInfo = new DTO_HomeCurrnetUserInfo
            {
                FullName = user?.FullName?? "",
                Role = user?.Roles ?? null
            };

            return currentUserInfo;
        }
        // ================ User Auth Management ================


        // ================ Password Management ================
        public async Task<ServiceResult> ChangePassword(int userId,ChangePasswordRequest request)
        {
            if (userId <= 0)
                return new(false, "Invalid user ID.", 400);


            if (!IsPasswordStrong(request.NewPassword))
                throw new Exception("Password must be at least 8 characters and contain letters, numbers, and special characters.");

            if (request == null || string.IsNullOrWhiteSpace(request.CurrentPassword) || string.IsNullOrWhiteSpace(request.NewPassword))
            {
                return new(false, "Passwords are required.", 400);
            }

            var user = await _authDLL.GetUserAuth(userId: userId);
            if (user == null)
                return new(false, "User not found.", 404);

            if (!user.IsActive)
                return new(false, "User account is inactive.", 403);

            if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
            {
                return new(false, "Current password is incorrect.", 400);
            }

            if (BCrypt.Net.BCrypt.Verify(request.NewPassword, user.PasswordHash))
            {
                return new(false, "New password must differ from current password.", 400);
            }

            var newHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

            var updated = await _authDLL.ChangePassword(userId, newHash);

            if (!updated)
                throw new Exception("Failed to update password.");

            await _auditLogService.LogAsync(userId, "PasswordChanged", "User", userId, new { PasswordChanged = false }, new { PasswordChanged = true });

            return new(true, "Password changed successfully.", 200);
        }
        public async Task<string> RequestResetPassword(RequestResetRequest request)
        {
            int userId = await _user.GetIdByPhoneOrEmail(request);
            if (userId <=0)
                throw new Exception("User not found");

            var token = new PasswordResetToken
            {
                UserId = userId,
                TokenHash = Guid.NewGuid().ToString(),
                ExpiresAt = DateTime.Now.AddMinutes(30),
                CreatedAt = DateTime.Now
            };

           if (!await _passwordResetTokenBLL.Add(token))
                throw new Exception("Failed to create password reset token");

            await _auditLogService.LogAsync(null, "PasswordResetRequested", "User", userId, null, new { ResetRequested = true });

            return token.TokenHash;
        }
        public async Task<bool> ResetPassword(ResetPasswordRequest request)
        {
            if (!IsPasswordStrong(request.NewPassword))
                throw new Exception("Password must be at least 8 characters and contain letters, numbers, and special characters.");

            var token = await _passwordResetTokenBLL.GetByToken(request.Token);
            if (token == null)
                throw new Exception("Invalid token");

            if (token.ExpiresAt < DateTime.Now)
                throw new Exception("Token expired");

            var user = await _authDLL.GetUserAuth(userId: token.UserId);
            if (user == null)
                throw new Exception("User not found");

            var newHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            if (!await _authDLL.ChangePassword(user.UserId, newHash))
                throw new Exception("Failed to update password");

            if (await _passwordResetTokenBLL.Delete(token.TokenId))
            {
                await _auditLogService.LogAsync(user.UserId, "PasswordReset", "User", user.UserId, new { PasswordChanged = false }, new { PasswordChanged = true });
                return true;
            }
            else
                throw new Exception("Failed to delete password reset token");
        }

        private int? GetCurrentUserId()
        {
            var value = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(value, out var userId) && userId > 0 ? userId : null;
        }
        private static bool IsPasswordStrong(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            return password.Length >= 8 &&
                   password.Any(char.IsLetter) &&
                   password.Any(char.IsDigit) &&
                   password.Any(c => !char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c));
        }
        // ================ Password Management ================
        /*
        // ================ Verification Management ================
          public async Task<bool> VerifyEmail(int userId)
        {
            return await _user.VerifyEmail(userId);
        }

        public async Task<bool> VerifyPhone(int userId)
        {
            return await _user.VerifyPhone(userId);
        } 
        // ================ Verification Management ================
         */
    }
}