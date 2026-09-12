using Data;
using Data.EF;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace DataLayer
{
    public class RefreshTokenDLL
    {
        public static async Task<int> Add(RefreshToken token)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                db.RefreshTokens.Add(token);
                await db.SaveChangesAsync();
                return token.TokenId;
            }
            catch (Exception ex)
            {
                WriteEventLog("Add RefreshToken Error", ex);
                return 0;
            }
        }

        public static async Task<RefreshToken?> GetByToken(string token)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token))
                    return null;

                using var db = new GamersPlatDbContext();

                var tokenHash = Convert.ToHexString(
                    SHA256.HashData(Encoding.UTF8.GetBytes(token)));

                return await db.RefreshTokens
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.TokenHash == tokenHash);
            }
            catch (Exception ex)
            {
                WriteEventLog("Get RefreshToken By Token Error", ex);
                return null;
            }
        }

        public static async Task<bool> Update(RefreshToken token)
        {
            try
            {
                if (token == null || token.TokenId <= 0)
                    return false;

                using var db = new GamersPlatDbContext();

                db.RefreshTokens.Update(token);
                return await db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                WriteEventLog("Update RefreshToken Error", ex);
                return false;
            }
        }

        private static void WriteEventLog(string title, Exception ex)
        {
            string error = ex.Message;
            if (ex.InnerException != null)
                error += "\nInner Exception: " + ex.InnerException.Message;
            EventLog.WriteEntry("Application", $"{title}: {error}", EventLogEntryType.Error);
        }
    }
}
