using Data;
using DataLayer;
namespace Bussiness
{
    public class UserRoleBLL
    {
        private enum enMode { AddMode = 1 }
        private enMode _mode = enMode.AddMode;

        private UserRole _ur;
        public int _urID = -1;

        public UserRoleBLL()
        {
            _ur = new UserRole();
            _mode = enMode.AddMode;
        }

        public UserRoleBLL(UserRole ur)
        {
            _ur = ur;
            _mode = enMode.AddMode;
        }

        public UserRole CurrentUserRole { get => _ur; set => _ur = value; }

        public bool Add()
        {
            _urID = UserRoleDLL.Add(_ur);
            return _urID > 0;
        }

        public static Task<List<UserRole>> GetByUserId(int userId) => UserRoleDLL.GetByUserId(userId);

        public bool Save() => _mode switch
        {
            enMode.AddMode => Add(),
            _ => false
        };
    }
}
