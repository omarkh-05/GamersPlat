using Data;
using Data.DLL;
using Data.EF;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace DataLayer
{
    public class PasswordResetTokenDLL
    {
        public static async Task<int> Add(PasswordResetToken token)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                db.PasswordResetTokens.Add(token);
                await db.SaveChangesAsync();
                return token.TokenId;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Add PasswordResetToken Error", ex);
                return 0;
            }
        }

        public static async Task<bool> Delete(int id)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = db.PasswordResetTokens.FirstOrDefault(t => t.TokenId == id);
                if (existing == null) return false;
                db.PasswordResetTokens.Remove(existing);
                return await db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Delete PasswordResetToken Error", ex);
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
                EventLog_Helper.WriteEventLog("Get PasswordResetToken By Token Error", ex);
                return null;
            }
        }


    }
}
