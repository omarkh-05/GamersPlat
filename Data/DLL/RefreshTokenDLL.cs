using Microsoft.EntityFrameworkCore;
using Data;
using Data.EF;
using System.Diagnostics;

namespace DataLayer
{
    public class RefreshTokenDLL
    {
        public static int Add(RefreshToken token)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                db.RefreshTokens.Add(token);
                db.SaveChanges();
                return token.TokenId;
            }
            catch (Exception ex)
            {
                WriteEventLog("Add RefreshToken Error", ex);
                return 0;
            }
        }

        public static bool Delete(int id)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = db.RefreshTokens.FirstOrDefault(t => t.TokenId == id);
                if (existing == null) return false;
                db.RefreshTokens.Remove(existing);
                return db.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                WriteEventLog("Delete RefreshToken Error", ex);
                return false;
            }
        }

        public static async Task<RefreshToken?> GetByToken(string token)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token)) return null;
                using var db = new GamersPlatDbContext();
                return await db.RefreshTokens.AsNoTracking().FirstOrDefaultAsync(t => t.TokenHash == token);
            }
            catch (Exception ex)
            {
                WriteEventLog("Get RefreshToken By Token Error", ex);
                return null;
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
