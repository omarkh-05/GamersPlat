using Bussiness.Interfaces;
using Data;
using DataLayer;
using Domain.DTOs.Auth;
using Domain.DTOs.User;
namespace Bussiness
{
    public class UserBLL : IUser
    {
        // ================ CRUD ================
        public async Task<bool> Add(User user)
        {
            var userId = await UserDLL.Add(user);
            return userId > 0;
        }
        public async Task<bool> Update(int userId, User updateUserInfoRequest)
        {
            return await UserDLL.Update(userId, updateUserInfoRequest);
        }
        public async Task<bool> Delete(int userId)
        {
            return await UserDLL.Delete(userId);
        }
        public async Task<List<DTO_UserListResponse>> GetAll()
        {
            return await UserDLL.GetAll();
        }
        // ================ CRUD ================


        // ================ Read By ================
        public async Task<DTO_UserInfoRequest?> GetUserInfoByID(int userId)
        {
            return await UserDLL.GetUserInfoByID(userId);
        }
        public async Task<DTO_UserInfoRequest?> GetUserInfoByPhone(string phone)
        {
            return await UserDLL.GetUserInfoByPhone(phone);
        }
        public async Task<User?> GetById(int userId)
        {
            return await UserDLL.GetById(userId);
        }
        public async Task<int> GetIdByPhoneOrEmail(RequestResetRequest reqResetPass)
        {
            return await UserDLL.GetIdByPhoneOrEmail(reqResetPass);
        }
        // ================ Read By ================


        // ================ Validation ============
        public async Task<bool> ExistsByEmail(string? email, int excludeId = 0)
        {
            return await UserDLL.ExistsByEmail(email, excludeId);
        }
        public async Task<bool> ExistsByPhone(string? phone, int excludeId = 0)
        {
            return await UserDLL.ExistsByPhone(phone, excludeId);
        }
        // ================ Validation ============
    }
}
