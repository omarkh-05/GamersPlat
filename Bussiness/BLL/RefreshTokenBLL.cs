using Data;
using DataLayer;
namespace Bussiness
{
    public class RefreshTokenBLL
    {
        // ================ CRUD ================
        public async Task<bool> Add(RefreshToken token)
        {
            return await RefreshTokenDLL.Add(token) > 0;
        }
        public async Task<bool> Update(RefreshToken token)
        {
            return await RefreshTokenDLL.Update(token);
        }
        // ================ CRUD ================


        // ================ Read By ================
        public async Task<RefreshToken?> GetByToken(string token) => await RefreshTokenDLL.GetByToken(token);
        // ================ Read By ================
    }
}
