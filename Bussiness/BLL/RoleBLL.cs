using Bussiness.Interfaces;
using Data;
using DataLayer;
namespace Bussiness
{
    public class RoleBLL: IRoles
    {
        //public async Task<bool> Add() => await RoleDLL.Add();
        //public async Task<bool> Update() => await RoleDLL.Update();
        //public async Task<bool> Delete(int roleId) => await RoleDLL.Delete(roleId);
        //public async Task<bool> ExistsByName(string? name, int excludeId = 0) => await RoleDLL.ExistsByName(name, excludeId);

        // ================ Read By ================
        public static async Task<Role?> GetByID(int roleId) => await RoleDLL.GetByID(roleId);
        public static async Task<Role?> GetByName(string name) => await RoleDLL.GetByName(name);
        // ================ Read By ================
    }
}
