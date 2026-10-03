using Data;
using DataLayer;

namespace Bussiness
{
    public class PasswordResetTokenBLL
    {
        private readonly PasswordResetTokenDLL _passwordResetTokenDLL;

        public PasswordResetTokenBLL(
            PasswordResetTokenDLL passwordResetTokenDLL)
        {
            _passwordResetTokenDLL = passwordResetTokenDLL;
        }

        // ================ CRUD ================
        public async Task<bool> Add(PasswordResetToken token)
        {
            int tokenID = await _passwordResetTokenDLL.Add(token);
            return tokenID > 0;
        }

        public async Task<bool> Delete(int id)
            => await _passwordResetTokenDLL.Delete(id);
        // ================ CRUD ================


        // ================ Read By ================
        public async Task<PasswordResetToken?> GetByToken(string token)
            => await _passwordResetTokenDLL.GetByToken(token);
        // ================ Read By ================
    }
}