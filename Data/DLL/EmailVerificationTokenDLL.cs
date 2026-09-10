using Microsoft.EntityFrameworkCore;
using Data;
using Data.EF;
using System.Diagnostics;

namespace DataLayer
{
    public class EmailVerificationTokenDLL
    {
        public static int Add(EmailVerificationToken token)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                db.EmailVerificationTokens.Add(token);
                db.SaveChanges();
                return token.TokenIdId;
            }
            catch (Exception ex)
            {
                WriteEventLog("Add EmailVerificationToken Error", ex);
                return 0;
            }
        }

        public static bool Delete(int id)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = db.EmailVerificationTokens.FirstOrDefault(t => t.TokenIdId == id);
                if (existing == null) return false;
                db.EmailVerificationTokens.Remove(existing);
                return db.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                WriteEventLog("Delete EmailVerificationToken Error", ex);
                return false;
            }
        }

        public static async Task<EmailVerificationToken?> GetByID(int id)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                return await db.EmailVerificationTokens.AsNoTracking().FirstOrDefaultAsync(t => t.TokenIdId == id);
            }
            catch (Exception ex)
            {
                WriteEventLog("Get EmailVerificationToken By ID Error", ex);
                return null;
            }
        }

        public static async Task<EmailVerificationToken?> GetByToken(string token)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token)) return null;
                using var db = new GamersPlatDbContext();
                return await db.EmailVerificationTokens.AsNoTracking().FirstOrDefaultAsync(t => t.TokenHash == token);
            }
            catch (Exception ex)
            {
                WriteEventLog("Get EmailVerificationToken By Token Error", ex);
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
