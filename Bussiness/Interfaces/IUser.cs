using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Data;

namespace Bussiness.Interfaces
{
    public interface IUser
    {
        Task<bool> Add(User user);
        Task<bool> Update(User user);
        Task<bool> Delete(int userId);

        Task<User?> GetByID(int userId);
        Task<List<User>> GetAll();
        Task<User?> GetByPhone(string phone);

        Task<bool> ExistsByEmail(string? email, int excludeId = 0);
        Task<bool> ExistsByPhone(string? phone, int excludeId = 0);
    }
}
