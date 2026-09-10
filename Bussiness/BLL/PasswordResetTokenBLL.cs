using Data;
using DataLayer;
namespace Bussiness
{
    public class PasswordResetTokenBLL
    {
        private enum enMode { AddMode = 1 }
        private enMode _mode = enMode.AddMode;

        private PasswordResetToken _token;
        public int _tokenID = -1;

        public PasswordResetTokenBLL()
        {
            _token = new PasswordResetToken();
            _mode = enMode.AddMode;
        }

        public PasswordResetTokenBLL(PasswordResetToken token)
        {
            _token = token;
            _mode = enMode.AddMode;
        }

        public PasswordResetToken CurrentToken { get => _token; set => _token = value; }

        public bool Add()
        {
            _tokenID = PasswordResetTokenDLL.Add(_token);
            return _tokenID > 0;
        }

        public bool Delete(int id) => PasswordResetTokenDLL.Delete(id);

        public static Task<PasswordResetToken?> GetByToken(string token) => PasswordResetTokenDLL.GetByToken(token);

        public bool Save() => _mode switch
        {
            enMode.AddMode => Add(),
            _ => false
        };
    }
}
