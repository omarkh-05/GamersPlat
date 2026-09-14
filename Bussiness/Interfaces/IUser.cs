using Data;
using Domain.DTOs.Auth;
using Domain.DTOs.User;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bussiness.Interfaces
{
    public interface IUser
    {
        Task<bool> Add(User user);
        Task<bool> Update(User user);
        Task<bool> Delete(int userId);

        Task<DTO_UserInfoRequest?> GetUserInfoByID(int userId);
        Task<List<DTO_UserListResponse>> GetAll();
        Task<DTO_UserInfoRequest?> GetUserInfoByPhone(string phone);
        Task<int> GetIdByPhoneOrEmail(RequestResetRequest reqResetPass);
        Task<User?> GetById(int userId);

        Task<bool> ExistsByEmail(string? email, int excludeId = 0);
        Task<bool> ExistsByPhone(string? phone, int excludeId = 0);
    }
}
