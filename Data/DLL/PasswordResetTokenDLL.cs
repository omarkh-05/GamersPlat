using Microsoft.EntityFrameworkCore;
using Data;
using Data.EF;
using System.Diagnostics;

namespace DataLayer
{
    public class PasswordResetTokenDLL
    {
        public static int Add(PasswordResetToken token)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                db.PasswordResetTokens.Add(token);
                db.SaveChanges();
                return token.TokenId;
            }
            catch (Exception ex)
            {
                WriteEventLog("Add PasswordResetToken Error", ex);
                return 0;
            }
        }

        public static bool Delete(int id)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = db.PasswordResetTokens.FirstOrDefault(t => t.TokenId == id);
                if (existing == null) return false;
                db.PasswordResetTokens.Remove(existing);
                return db.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                WriteEventLog("Delete PasswordResetToken Error", ex);
                return false;
            }
        }

        public static async Task<PasswordResetToken?> GetByToken(string token)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token)) return null;
                using var db = new GamersPlatDbContext();
                return await db.PasswordResetTokens.AsNoTracking().FirstOrDefaultAsync(t => t.TokenHash == token);
            }
            catch (Exception ex)
            {
                WriteEventLog("Get PasswordResetToken By Token Error", ex);
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
