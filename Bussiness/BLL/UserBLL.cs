using Bussiness.Interfaces;
using Data;
using DataLayer;
namespace Bussiness
{
    public class UserBLL : IUser
    {
        public async Task<bool> Add(User user)
        {
            var userId = await UserDLL.Add(user);
            return userId > 0;
        }

        public async Task<bool> Update(User user)
        {
            return await UserDLL.Update(user);
        }

        public async Task<bool> Delete(int userId)
        {
            return await UserDLL.Delete(userId);
        }

        public async Task<User?> GetByID(int userId)
        {
            return await UserDLL.GetByID(userId);
        }

        public async Task<List<User>> GetAll()
        {
            return await UserDLL.GetAll();
        }

        public async Task<User?> GetByPhone(string phone)
        {
            return await UserDLL.GetByPhone(phone);
        }

        public async Task<bool> ExistsByEmail(string? email, int excludeId = 0)
        {
            return await UserDLL.ExistsByEmail(email, excludeId);
        }

        public async Task<bool> ExistsByPhone(string? phone, int excludeId = 0)
        {
            return await UserDLL.ExistsByPhone(phone, excludeId);
        }
    }
}
