using Azure.Core;
using Bussiness.Helpers;
using Bussiness.Interfaces;
using Data;
using Data.DLL;
using Domain.DTOs.Auth;
using System.Security.Cryptography;
using System.Text;

namespace Bussiness.BLL
{
    public class AuthBLL : IAuthService
    {
        private readonly IUser _user;
        private readonly AuthHelpers _authHelpers;
        private readonly RefreshTokenBLL _refreshTokenBLL;
        private readonly IRoles _roles;
        private readonly IUserRole _userRole;
        private readonly PasswordResetTokenBLL _passwordResetTokenBLL;
        public AuthBLL(IUser user,AuthHelpers authHelpers, RefreshTokenBLL refreshTokenBLL, IRoles roles, IUserRole userRole, PasswordResetTokenBLL passwordResetTokenBLL)
        {
            _authHelpers = authHelpers;
            _refreshTokenBLL = refreshTokenBLL;
            _user = user;
            _roles = roles;
            _userRole = userRole;
            _passwordResetTokenBLL = passwordResetTokenBLL;
        }

        // ================ User Auth Management ================
        public async Task RegisterWithRoleAsync(UserRegisterRequest request, string roleName)
        {
            if (string.IsNullOrWhiteSpace(request.FullName) ||
                string.IsNullOrWhiteSpace(request.PhoneNumber) ||
                string.IsNullOrWhiteSpace(request.Password))
                throw new Exception("Invalid data");

            if (await _user.ExistsByPhone(request.PhoneNumber))
                throw new Exception("Phone number already exists");

            var role = await _roles.GetByName(roleName); // "Player" أو "CenterOwner"
            if (role == null)
                throw new Exception("Invalid role");

            
                var user = new User
                {
                    FullName = request.FullName,
                    PhoneNumber = request.PhoneNumber,
                    Email = request.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
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
        }

        public async Task<TokenResponse> LoginAsync(LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.PhoneNumber) ||
                string.IsNullOrWhiteSpace(request.Password))
                throw new Exception("Invalid data");

            var user = await AuthDLL.GetUserAuthByPhone(request.PhoneNumber);

            if (user == null || !user.IsActive)
                throw new UnauthorizedAccessException("Invalid credentials");

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid credentials");

            var fullUser = await _user.GetById(user.UserId);
            if (fullUser == null || !fullUser.IsActive)
                throw new UnauthorizedAccessException("Invalid credentials");

            var accessToken = _authHelpers.GenerateAccessToken(fullUser);
            var refreshToken = _authHelpers.GenerateRefreshToken();

            var rt = new RefreshToken
            {
                UserId = user.UserId,
                TokenHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken))),
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
                IsRevoked = false
            };

            if (!await _refreshTokenBLL.Add(rt))
                throw new Exception("Failed to create refresh token");

            return new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
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
                return true;

            existingToken.IsRevoked = true;
            existingToken.RevokedAt = DateTime.UtcNow;

            return await _refreshTokenBLL.Update(existingToken);
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

            var user = await AuthDLL.GetUserAuthByID(existingToken.UserId);

            if (user == null || !user.IsActive)
                throw new UnauthorizedAccessException("Invalid refresh request");

            var fullUser = await _user.GetById(user.UserId);
            if (fullUser == null || !fullUser.IsActive)
                throw new UnauthorizedAccessException("Invalid refresh request");

            var newAccessToken = _authHelpers.GenerateAccessToken(fullUser);
            var newRefreshToken = _authHelpers.GenerateRefreshToken();

            existingToken.IsRevoked = true;
            existingToken.RevokedAt = DateTime.UtcNow;
             
            if (!await _refreshTokenBLL.Update(existingToken))
                throw new Exception("Failed to revoke refresh token");

            var rt = new RefreshToken
            {
                UserId = user.UserId,
                TokenHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(newRefreshToken))),
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
                IsRevoked = false
            };

            if (!await _refreshTokenBLL.Add(rt))
                throw new Exception("Failed to create refresh token");

            return new TokenResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }
        // ================ User Auth Management ================


        // ================ Password Management ================
        public async Task<bool> ChangePassword(int userId, ChangePasswordRequest request)
        {
            if (userId <= 0 ||
                string.IsNullOrWhiteSpace(request.OldPassword) ||
                string.IsNullOrWhiteSpace(request.NewPassword))
                return false;

            var user = await AuthDLL.GetUserAuthByID(userId);

            if (user == null || !user.IsActive)
                return false;

            if (!BCrypt.Net.BCrypt.Verify(request.OldPassword, user.PasswordHash))
                return false;

            var newHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

            if (await AuthDLL.ChangePassword(userId, newHash)) return true;
            else
                throw new Exception("Error updating user");
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
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                CreatedAt = DateTime.UtcNow
            };

           if (!await _passwordResetTokenBLL.Add(token))
                throw new Exception("Failed to create password reset token");

            return token.TokenHash;
        }

        public async Task<bool> ResetPassword(ResetPasswordRequest request)
        {
            var token = await _passwordResetTokenBLL.GetByToken(request.Token);
            if (token == null)
                throw new Exception("Invalid token");

            if (token.ExpiresAt < DateTime.UtcNow)
                throw new Exception("Token expired");

            var user = await AuthDLL.GetUserAuthByID(token.UserId);
            if (user == null)
                throw new Exception("User not found");

            var newHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            if (!await AuthDLL.ChangePassword(user.UserId, newHash))
                throw new Exception("Failed to update password");

            if (await _passwordResetTokenBLL.Delete(token.TokenId))
                return true;
            else
                throw new Exception("Failed to delete password reset token");
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