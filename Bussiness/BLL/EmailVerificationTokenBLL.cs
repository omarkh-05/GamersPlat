using Data;
using DataLayer;

namespace Bussiness
{
    public class EmailVerificationTokenBLL
    {
        private readonly EmailVerificationTokenDLL _emailVerificationTokenDLL;

        public EmailVerificationTokenBLL(
            EmailVerificationTokenDLL emailVerificationTokenDLL)
        {
            _emailVerificationTokenDLL = emailVerificationTokenDLL;
        }

        // ================ CRUD ================
        public async Task<bool> Add(EmailVerificationToken token)
        {
            int tokenID = await _emailVerificationTokenDLL.Add(token);
            return tokenID > 0;
        }

        public async Task<bool> Delete(int id)
            => await _emailVerificationTokenDLL.Delete(id);
        // ================ CRUD ================


        // ================ Read By ================
        public async Task<EmailVerificationToken?> GetByID(int id)
            => await _emailVerificationTokenDLL.GetByID(id);

        public async Task<EmailVerificationToken?> GetByToken(string token)
            => await _emailVerificationTokenDLL.GetByToken(token);
        // ================ Read By ================
    }
}