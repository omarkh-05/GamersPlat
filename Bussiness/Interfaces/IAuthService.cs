using Data;
using Domain.DTOs.Auth;
using Domain.DTOs.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bussiness.Interfaces
{
    public interface IAuthService
    {
        Task<TokenResponse> LoginAsync(LoginRequest request);
        Task RegisterWithRoleAsync(UserRegisterRequest request, string roleName);
        Task<TokenResponse> RefreshAsync(string refreshToken);
        Task<bool> LogoutAsync(string refreshToken);
        Task<DTO_UserInfoRequest?> CheckAuth(string phoneNumber);
        Task<bool> ChangePassword(int userId, ChangePasswordRequest request);
        Task<string> RequestResetPassword(RequestResetRequest request);
        Task<bool> ResetPassword(ResetPasswordRequest request);


        // Task<bool> ResetPassword(int userId,string newPassword);
        // Task<bool> VerifyEmail(int userId);
        // Task<bool> VerifyPhone(int userId);
    }
}
