using Bussiness.Interfaces;
using Data;
using DataLayer;
using Domain.DTOs.Auth;
using Domain.DTOs.User;
namespace Bussiness
{
    public class UserBLL : IUser
    {
        private readonly UserDLL _userDLL;

        public UserBLL(UserDLL userDLL)
        {
            _userDLL = userDLL;
        }

        // ================ CRUD ================
        public async Task<bool> Add(User user)
        {
            var userId = await _userDLL.Add(user);
            return userId > 0;
        }
        public async Task<bool> Update(int userId, User updateUserInfoRequest)
        {
            return await _userDLL.Update(userId, updateUserInfoRequest);
        }
        public async Task<bool> Delete(int userId)
        {
            return await _userDLL.Delete(userId);
        }
        public async Task<List<DTO_UserListResponse>> GetAll()
        {
            return await _userDLL.GetAll();
        }
        // ================ CRUD ================


        // ================ Read By ================
        public async Task<DTO_UserInfoRequest?> GetUserInfoByID(int userId)
        {
            return await _userDLL.GetUserInfoByID(userId);
        }
        public async Task<DTO_UserInfoRequest?> GetUserInfoByPhone(string phone)
        {
            return await _userDLL.GetUserInfoByPhone(phone);
        }
        public async Task<User?> GetById(int userId)
        {
            return await _userDLL.GetById(userId);
        }
        public async Task<int> GetIdByPhoneOrEmail(RequestResetRequest reqResetPass)
        {
            return await _userDLL.GetIdByPhoneOrEmail(reqResetPass);
        }
        // ================ Read By ================


        // ================ Validation ============
        public async Task<bool> ExistsByEmail(string? email, int excludeId = 0)
        {
            return await _userDLL.ExistsByEmail(email, excludeId);
        }
        public async Task<bool> ExistsByPhone(string? phone, int excludeId = 0)
        {
            return await _userDLL.ExistsByPhone(phone, excludeId);
        }
        // ================ Validation ============
    }
}
