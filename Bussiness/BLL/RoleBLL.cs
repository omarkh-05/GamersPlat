using Data;
using DataLayer;
namespace Bussiness
{
    public class RoleBLL
    {
        private enum enMode { AddMode = 1, UpdateMode = 2 }
        private enMode _mode = enMode.AddMode;

        private Role _role;
        public int _roleID = -1;

        public RoleBLL()
        {
            _role = new Role();
            _mode = enMode.AddMode;
        }

        public RoleBLL(Role role)
        {
            _role = role;
            _mode = enMode.UpdateMode;
        }

        public Role CurrentRole { get => _role; set => _role = value; }

        public bool Add()
        {
            _roleID = RoleDLL.Add(_role);
            return _roleID > 0;
        }

        public bool Update() => RoleDLL.Update(_role);

        public bool Delete(int roleId) => RoleDLL.Delete(roleId);

        public static Task<Role?> GetByID(int roleId) => RoleDLL.GetByID(roleId);

        public static bool ExistsByName(string? name, int excludeId = 0) => RoleDLL.ExistsByName(name, excludeId);

        public bool Save() => _mode switch
        {
            enMode.AddMode => Add(),
            enMode.UpdateMode => Update(),
            _ => false
        };
    }
}
