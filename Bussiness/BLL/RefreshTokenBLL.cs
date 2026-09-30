using Data;
using DataLayer;

namespace Bussiness
{
    public class RefreshTokenBLL
    {
        private readonly RefreshTokenDLL _refreshTokenDLL;

        public RefreshTokenBLL(RefreshTokenDLL refreshTokenDLL)
        {
            _refreshTokenDLL = refreshTokenDLL;
        }

        // ================ CRUD ================
        public async Task<bool> Add(RefreshToken token)
        {
            return await _refreshTokenDLL.Add(token) > 0;
        }

        public async Task<bool> Update(RefreshToken token)
        {
            return await _refreshTokenDLL.Update(token);
        }
        // ================ CRUD ================


        // ================ Read By ================
        public async Task<RefreshToken?> GetByToken(string token)
            => await _refreshTokenDLL.GetByToken(token);
        // ================ Read By ================
    }
}