using Bussiness.Interfaces;
using Data;
using DataLayer;
namespace Bussiness
{
    public class UserRoleBLL: IUserRole
    {
        // ================ CRUD ================
        public async Task<bool> Add(UserRole _ur)
        {
           int _urID = await UserRoleDLL.Add(_ur);
            return _urID > 0;
        }
        // ================ CRUD ================


        // ================ Read By ================
        public async Task<List<UserRole>> GetByUserId(int userId) => await UserRoleDLL.GetByUserId(userId);
        // ================ Read By ================
    }
}
