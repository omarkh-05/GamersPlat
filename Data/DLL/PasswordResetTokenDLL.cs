using Data;
using Data.DLL;
using Data.EF;
using Microsoft.EntityFrameworkCore;

namespace DataLayer
{
    public class PasswordResetTokenDLL
    {
        private readonly GamersPlatDbContext _db;

        public PasswordResetTokenDLL(GamersPlatDbContext db)
        {
            _db = db;
        }

        // ================ CRUD ================
        public async Task<int> Add(PasswordResetToken token)
        {
            try
            {
                _db.PasswordResetTokens.Add(token);
                await _db.SaveChangesAsync();

                return token.TokenId;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog(
                    "Add PasswordResetToken Error",
                    ex);

                return 0;
            }
        }

        public async Task<bool> Delete(int id)
        {
            try
            {
                var existing = await _db.PasswordResetTokens
                    .FirstOrDefaultAsync(t => t.TokenId == id);

                if (existing == null)
                    return false;

                _db.PasswordResetTokens.Remove(existing);

                return await _db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog(
                    "Delete PasswordResetToken Error",
                    ex);

                return false;
            }
        }
        // ================ CRUD ================


        // ================ Read By ================
        public async Task<PasswordResetToken?> GetByToken(string token)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token))
                    return null;

                return await _db.PasswordResetTokens
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.TokenHash == token);
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog(
                    "Get PasswordResetToken By Token Error",
                    ex);

                return null;
            }
        }
        // ================ Read By ================
    }
}