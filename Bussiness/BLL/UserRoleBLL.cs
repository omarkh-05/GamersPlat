using Bussiness.Interfaces;
using Data;
using DataLayer;

namespace Bussiness
{
    public class UserRoleBLL : IUserRole
    {
        private readonly UserRoleDLL _userRoleDLL;

        public UserRoleBLL(UserRoleDLL userRoleDLL)
        {
            _userRoleDLL = userRoleDLL;
        }

        // ================ CRUD ================
        public async Task<bool> Add(UserRole _ur)
        {
            int _urID = await _userRoleDLL.Add(_ur);
            return _urID > 0;
        }
        // ================ CRUD ================


        // ================ Read By ================
        public async Task<List<UserRole>> GetByUserId(int userId)
            => await _userRoleDLL.GetByUserId(userId);

        // ================ Read By ================
    }
}