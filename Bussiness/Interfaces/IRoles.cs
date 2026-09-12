using Data;
using DataLayer;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bussiness.Interfaces
{
    public interface IRoles
    {
        Task<Role?> GetByID(int roleId) => RoleDLL.GetByID(roleId);
        Task<Role?> GetByName(string name) => RoleDLL.GetByName(name);
    }
}
