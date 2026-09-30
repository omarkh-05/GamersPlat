using Data;
using Data.DLL;
using Data.EF;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace DataLayer
{
    public class RefreshTokenDLL
    {
        private readonly GamersPlatDbContext _db;

        public RefreshTokenDLL(GamersPlatDbContext db)
        {
            _db = db;
        }

        // ================ CRUD ================
        public async Task<int> Add(RefreshToken token)
        {
            try
            {
                _db.RefreshTokens.Add(token);
                await _db.SaveChangesAsync();
                return token.TokenId;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Add RefreshToken Error", ex);
                return 0;
            }
        }

        public async Task<bool> Update(RefreshToken token)
        {
            try
            {
                if (token == null || token.TokenId <= 0)
                    return false;

                _db.RefreshTokens.Update(token);
                return await _db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Update RefreshToken Error", ex);
                return false;
            }
        }
        // ================ CRUD ================


        // ================ Read By ================
        public async Task<RefreshToken?> GetByToken(string token)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token))
                    return null;

                var tokenHash = Convert.ToHexString(
                    SHA256.HashData(Encoding.UTF8.GetBytes(token)));

                return await _db.RefreshTokens
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.TokenHash == tokenHash);
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get RefreshToken By Token Error", ex);
                return null;
            }
        }
        // ================ Read By ================
    }
}