using Data;
using DataLayer;
namespace Bussiness
{
    public class EmailVerificationTokenBLL
    {
        // ================ CRUD ================
        public async Task<bool> Add(EmailVerificationToken token)
        {
            int tokenID = await EmailVerificationTokenDLL.Add(token);
            return tokenID > 0;
        }
        public async Task<bool> Delete(int id) => await EmailVerificationTokenDLL.Delete(id);
        // ================ CRUD ================


        // ================ Read By ================
        public async Task<EmailVerificationToken?> GetByID(int id) => await EmailVerificationTokenDLL.GetByID(id);
        public async Task<EmailVerificationToken?> GetByToken(string token) => await EmailVerificationTokenDLL.GetByToken(token);
        // ================ Read By ================
    }
}
