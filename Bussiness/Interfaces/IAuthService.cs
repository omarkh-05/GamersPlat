using Data;
using Domain.DTOs.Auth;
using Domain.DTOs.User;
using System;
using System.Collections.Generic;
using System.Text;
using static Bussiness.BLL.AuthBLL;

namespace Bussiness.Interfaces
{
    public interface IAuthService
    {
        Task<TokenResponse> LoginAsync(LoginRequest request);
        Task<int> RegisterWithRoleAsync(UserRegisterRequest request, string roleName);
        Task<TokenResponse> RefreshAsync(string refreshToken);
        Task<bool> LogoutAsync(string refreshToken);
        Task<DTO_HomeCurrnetUserInfo?> CheckAuth(string phoneNumber);
        Task<ServiceResult> ChangePassword(int userId, ChangePasswordRequest request);
        Task<string> RequestResetPassword(RequestResetRequest request);
        Task<bool> ResetPassword(ResetPasswordRequest request);


        // Task<bool> ResetPassword(int userId,string newPassword);
        // Task<bool> VerifyEmail(int userId);
        // Task<bool> VerifyPhone(int userId);
    }
}
