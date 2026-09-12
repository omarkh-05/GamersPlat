using Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bussiness.Interfaces
{
    public interface IUserRole
    {
        Task<bool> Add(UserRole _ur);
        Task<List<UserRole>> GetByUserId(int userId);
    }
}
