using Data;
using DataLayer;
namespace Bussiness
{
    public class EmailVerificationTokenBLL
    {
        // ================ CRUD ================
        public bool Add(EmailVerificationToken token)
        {
            int tokenID = EmailVerificationTokenDLL.Add(token);
            return tokenID > 0;
        }
        public bool Delete(int id) => EmailVerificationTokenDLL.Delete(id);
        // ================ CRUD ================


        // ================ Read By ================
        public static Task<EmailVerificationToken?> GetByID(int id) => EmailVerificationTokenDLL.GetByID(id);
        public static Task<EmailVerificationToken?> GetByToken(string token) => EmailVerificationTokenDLL.GetByToken(token);
        // ================ Read By ================
    }
}
