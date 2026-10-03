using Data;
using Data.DLL;
using Data.EF;
using Microsoft.EntityFrameworkCore;

namespace DataLayer
{
    public class EmailVerificationTokenDLL
    {
        private readonly GamersPlatDbContext _db;

        public EmailVerificationTokenDLL(GamersPlatDbContext db)
        {
            _db = db;
        }

        // ================ CRUD ================
        public async Task<int> Add(EmailVerificationToken token)
        {
            try
            {
                _db.EmailVerificationTokens.Add(token);
                await _db.SaveChangesAsync();

                return token.TokenIdId;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog(
                    "Add EmailVerificationToken Error",
                    ex);

                return 0;
            }
        }

        public async Task<bool> Delete(int id)
        {
            try
            {
                var existing = await _db.EmailVerificationTokens
                    .FirstOrDefaultAsync(t => t.TokenIdId == id);

                if (existing == null)
                    return false;

                _db.EmailVerificationTokens.Remove(existing);

                return await _db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog(
                    "Delete EmailVerificationToken Error",
                    ex);

                return false;
            }
        }
        // ================ CRUD ================


        // ================ Read By ================
        public async Task<EmailVerificationToken?> GetByID(int id)
        {
            try
            {
                return await _db.EmailVerificationTokens
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.TokenIdId == id);
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog(
                    "Get EmailVerificationToken By ID Error",
                    ex);

                return null;
            }
        }

        public async Task<EmailVerificationToken?> GetByToken(string token)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token))
                    return null;

                return await _db.EmailVerificationTokens
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.TokenHash == token);
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog(
                    "Get EmailVerificationToken By Token Error",
                    ex);

                return null;
            }
        }
        // ================ Read By ================
    }
}