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

        public async Task<bool> RotateAsync(int tokenId, string tokenHash, DateTime now, RefreshToken replacement)
        {
            await using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var revoked = await _db.RefreshTokens
                    .Where(t => t.TokenId == tokenId
                        && t.TokenHash == tokenHash
                        && !t.IsRevoked
                        && t.RevokedAt == null
                        && t.ExpiresAt > now)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(t => t.IsRevoked, true)
                        .SetProperty(t => t.RevokedAt, now));

                if (revoked != 1)
                {
                    await transaction.RollbackAsync();
                    return false;
                }

                _db.RefreshTokens.Add(replacement);
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                try { await transaction.RollbackAsync(); } catch { }
                EventLog_Helper.WriteEventLog("Rotate RefreshToken Error", ex);
                throw;
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