using Azure.Core;
using Bussiness.Helpers;
using Bussiness.Interfaces;
using Data;
using DataLayer;
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
        public AuthBLL(IUser user,AuthHelpers authHelpers, RefreshTokenBLL refreshTokenBLL)
        {
            _authHelpers = authHelpers;
            _refreshTokenBLL = refreshTokenBLL;
            _user = user;
        }

        public async Task RegisterAsync(RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.FullName) ||
                string.IsNullOrWhiteSpace(request.PhoneNumber) ||
                string.IsNullOrWhiteSpace(request.Password))
                throw new Exception("Invalid data");

            if (await _user.ExistsByPhone(request.PhoneNumber))
                throw new Exception("Phone number already exists");

            var user = new User
            {
                FullName = request.FullName,
                PhoneNumber = request.PhoneNumber,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                IsActive = true,
                CityId = request.CityId,
                CreatedAt = DateTime.UtcNow
            };

            if (!await _user.Add(user))
                throw new Exception("Failed to create user");
        }

        public async Task<TokenResponse> LoginAsync(LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.PhoneNumber) ||
                string.IsNullOrWhiteSpace(request.Password))
                throw new Exception("Invalid data");

            var user = await _user.GetByPhone(request.PhoneNumber);

            if (user == null || !user.IsActive)
                throw new UnauthorizedAccessException("Invalid credentials");

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid credentials");

            var accessToken = _authHelpers.GenerateAccessToken(user);
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

            var user = await _user.GetByID(existingToken.UserId);

            if (user == null || !user.IsActive)
                throw new UnauthorizedAccessException("Invalid refresh request");

            var newAccessToken = _authHelpers.GenerateAccessToken(user);
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

        public async Task<bool> ChangePassword(int userId,string currentPassword, string newPassword)
        {
            if (userId <= 0 ||
                string.IsNullOrWhiteSpace(currentPassword) ||
                string.IsNullOrWhiteSpace(newPassword))
                return false;

            var user = await _user.GetByID(userId);

            if (user == null || !user.IsActive)
                return false;

            if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
                return false;

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);

            return await _user.Update(user);
        }

        /* public async Task<bool> VerifyEmail(int userId)
        {
            return await _user.VerifyEmail(userId);
        }

        public async Task<bool> VerifyPhone(int userId)
        {
            return await _user.VerifyPhone(userId);
        } */
    }
}