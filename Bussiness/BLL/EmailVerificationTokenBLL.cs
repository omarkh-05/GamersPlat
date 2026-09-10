using Data;
using DataLayer;
namespace Bussiness
{
    public class EmailVerificationTokenBLL
    {
        private enum enMode { AddMode = 1 }
        private enMode _mode = enMode.AddMode;

        private EmailVerificationToken _token;
        public int _tokenID = -1;

        public EmailVerificationTokenBLL()
        {
            _token = new EmailVerificationToken();
            _mode = enMode.AddMode;
        }

        public EmailVerificationTokenBLL(EmailVerificationToken token)
        {
            _token = token;
            _mode = enMode.AddMode;
        }

        public EmailVerificationToken CurrentToken { get => _token; set => _token = value; }

        public bool Add()
        {
            _tokenID = EmailVerificationTokenDLL.Add(_token);
            return _tokenID > 0;
        }

        public bool Delete(int id) => EmailVerificationTokenDLL.Delete(id);

        public static Task<EmailVerificationToken?> GetByID(int id) => EmailVerificationTokenDLL.GetByID(id);

        public static Task<EmailVerificationToken?> GetByToken(string token) => EmailVerificationTokenDLL.GetByToken(token);

        public bool Save() => _mode switch
        {
            enMode.AddMode => Add(),
            _ => false
        };
    }
}
