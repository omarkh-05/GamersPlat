using Data;
using DataLayer;
namespace Bussiness
{
    public class UserBLL
    {
        private enum enMode { AddMode = 1, UpdateMode = 2 }
        private enMode _mode = enMode.AddMode;

        private User _user;
        public int _userID = -1;

        public UserBLL()
        {
            _user = new User();
            _mode = enMode.AddMode;
        }

        public UserBLL(User user)
        {
            _user = user;
            _mode = enMode.UpdateMode;
        }

        public User CurrentUser { get => _user; set => _user = value; }

        public bool Add()
        {
            _userID = UserDLL.Add(_user);
            return _userID > 0;
        }

        public bool Update() => UserDLL.Update(_user);

        public bool Delete(int userId) => UserDLL.Delete(userId);

        public static Task<User?> GetByID(int userId) => UserDLL.GetByID(userId);

        public static Task<List<User>> GetAll() => UserDLL.GetAll();

        public static bool ExistsByEmail(string? email, int excludeId = 0) => UserDLL.ExistsByEmail(email, excludeId);

        public static bool ExistsByPhone(string? phone, int excludeId = 0) => UserDLL.ExistsByPhone(phone, excludeId);

        public bool Save() => _mode switch
        {
            enMode.AddMode => Add(),
            enMode.UpdateMode => Update(),
            _ => false
        };
    }
}
