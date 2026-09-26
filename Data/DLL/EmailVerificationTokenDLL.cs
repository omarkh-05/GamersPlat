using Data;
using Data.DLL;
using Data.EF;
using Microsoft.EntityFrameworkCore;

namespace DataLayer
{
    public class EmailVerificationTokenDLL
    {
        // ================ CRUD ================
        public static async Task<int> Add(EmailVerificationToken token)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                db.EmailVerificationTokens.Add(token);
                await db.SaveChangesAsync();
                return token.TokenIdId;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Add EmailVerificationToken Error", ex);
                return 0;
            }
        }
        public static async Task<bool> Delete(int id)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = await db.EmailVerificationTokens.FirstOrDefaultAsync(t => t.TokenIdId == id);
                if (existing == null) return false;
                db.EmailVerificationTokens.Remove(existing);
                return await db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Delete EmailVerificationToken Error", ex);
                return false;
            }
        }
        // ================ CRUD ================


        // ================ Read By ================
        public static async Task<EmailVerificationToken?> GetByID(int id)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                return await db.EmailVerificationTokens.AsNoTracking().FirstOrDefaultAsync(t => t.TokenIdId == id);
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get EmailVerificationToken By ID Error", ex);
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
                EventLog_Helper.WriteEventLog("Get EmailVerificationToken By Token Error", ex);
                return null;
            }
        }
        // ================ Read By ================
    }
}
