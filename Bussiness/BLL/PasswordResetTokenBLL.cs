using Data;
using DataLayer;
namespace Bussiness
{
    public class PasswordResetTokenBLL
    {
        // ================ Crud ================
        public async Task<bool> Add(PasswordResetToken _token)
        {
            int _tokenID = await PasswordResetTokenDLL.Add(_token);
            return _tokenID > 0;
        }
        public async Task< bool> Delete(int id) => await PasswordResetTokenDLL.Delete(id);
        // ================ Crud ================


        // ================ Read By ================
        public async Task<PasswordResetToken?> GetByToken(string token) => await PasswordResetTokenDLL.GetByToken(token);
        // ================ Read By ================
    }
}
