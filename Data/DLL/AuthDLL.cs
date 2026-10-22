using Data.EF;
using Domain.DTOs.Auth;
using Microsoft.EntityFrameworkCore;

namespace Data.DLL
{
    public class AuthDLL
    {
        private readonly GamersPlatDbContext _db;

        public AuthDLL(GamersPlatDbContext db)
        {
            _db = db;
        }

        // ================ Read By For Auth ================
        public async Task<DTO_UserAuthInfo?> GetUserAuth(string? phoneNumber = null,int? userId = null)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber) && userId == null)
                return null;
            try
            {
                var query = _db.Users.AsQueryable();

                if (userId.HasValue)
                    query = query.Where(u => u.UserId == userId.Value);
                else
                    query = query.Where(u => u.PhoneNumber == phoneNumber);

                return await query
                    .Select(u => new DTO_UserAuthInfo
                    {
                        UserId = u.UserId,
                        PasswordHash = u.PasswordHash,
                        PhoneNumber = u.PhoneNumber,
                        Email = u.Email,
                        FullName = u.FullName,
                        Role = u.UserRoles.Select(ur => ur.Role.RoleName).ToList(),
                        IsActive = u.IsActive
                    })
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get User Auth Error", ex);
                return null;
            }
        }
        // ================ Read By For Auth ================


        // ================ Security ================
        public async Task<bool> ChangePassword(int userId, string newPasswordHash)
        {
            try
            {
                var existing = await _db.Users.FindAsync(userId);

                if (existing == null) return false;

                existing.PasswordHash = newPasswordHash;
                existing.UpdatedAt = DateTime.Now;

                return await _db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Update Password Error", ex);
                return false;
            }
        }

        public async Task<bool> VerifyEmail(int userId)
        {
            try
            {
                var existing = await _db.Users.FindAsync(userId);

                if (existing == null) return false;

                existing.EmailVerified = true;
                existing.UpdatedAt = DateTime.UtcNow;

                return await _db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Verify Email Error", ex);
                return false;
            }
        }

        public async Task<bool> VerifyPhone(int userId)
        {
            try
            {
                var existing = await _db.Users.FindAsync(userId);

                if (existing == null) return false;

                existing.PhoneVerified = true;
                existing.UpdatedAt = DateTime.UtcNow;

                return await _db.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Verify Phone Error", ex);
                return false;
            }
        }

        // ================ Security ================
    }
}