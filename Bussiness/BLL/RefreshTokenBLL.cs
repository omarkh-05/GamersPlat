using Data;
using DataLayer;
namespace Bussiness
{
    public class RefreshTokenBLL
    {
        public async Task<bool> Add(RefreshToken token)
        {
            return await RefreshTokenDLL.Add(token) > 0;
        }

        public async Task<bool> Update(RefreshToken token)
        {
            return await RefreshTokenDLL.Update(token);
        }

        public Task<RefreshToken?> GetByToken(string token) => RefreshTokenDLL.GetByToken(token);

    }
}
