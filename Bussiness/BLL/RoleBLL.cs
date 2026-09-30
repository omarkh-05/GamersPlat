using Bussiness.Interfaces;
using Data;
using DataLayer;
namespace Bussiness
{
    public class RoleBLL: IRoles
    {
        private readonly RoleDLL _roleDLL;

        public RoleBLL(RoleDLL roleDLL)
        {
            _roleDLL = roleDLL;
        }
        //public async Task<bool> Add() => await RoleDLL.Add();
        //public async Task<bool> Update() => await RoleDLL.Update();
        //public async Task<bool> Delete(int roleId) => await RoleDLL.Delete(roleId);
        //public async Task<bool> ExistsByName(string? name, int excludeId = 0) => await RoleDLL.ExistsByName(name, excludeId);

            // ================ Read By ================
        public async Task<Role?> GetByID(int roleId) => await _roleDLL.GetByID(roleId);
        public async Task<Role?> GetByName(string name) => await _roleDLL.GetByName(name);
        // ================ Read By ================
    }
}
