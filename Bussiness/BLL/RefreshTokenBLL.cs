using Data;
using DataLayer;
namespace Bussiness
{
    public class RefreshTokenBLL
    {
        private enum enMode { AddMode = 1 }
        private enMode _mode = enMode.AddMode;

        private RefreshToken _token;
        public int _tokenID = -1;

        public RefreshTokenBLL()
        {
            _token = new RefreshToken();
            _mode = enMode.AddMode;
        }

        public RefreshTokenBLL(RefreshToken token)
        {
            _token = token;
            _mode = enMode.AddMode;
        }

        public RefreshToken CurrentToken { get => _token; set => _token = value; }

        public bool Add()
        {
            _tokenID = RefreshTokenDLL.Add(_token);
            return _tokenID > 0;
        }

        public bool Delete(int id) => RefreshTokenDLL.Delete(id);

        public static Task<RefreshToken?> GetByToken(string token) => RefreshTokenDLL.GetByToken(token);

        public bool Save() => _mode switch
        {
            enMode.AddMode => Add(),
            _ => false
        };
    }
}
