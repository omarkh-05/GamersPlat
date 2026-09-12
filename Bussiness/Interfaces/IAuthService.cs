using Data;
using Domain.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bussiness.Interfaces
{
    public interface IAuthService
    {
        Task<TokenResponse> LoginAsync(LoginRequest request);
        Task RegisterAsync(RegisterRequest request);
        Task<TokenResponse> RefreshAsync(string refreshToken);
        Task<bool> LogoutAsync(string refreshToken);
        Task<bool> ChangePassword(int userId, string currentPassword, string newPassword);

        // Task<bool> ResetPassword(int userId,string newPassword);
        // Task<bool> VerifyEmail(int userId);
        // Task<bool> VerifyPhone(int userId);
    }
}
