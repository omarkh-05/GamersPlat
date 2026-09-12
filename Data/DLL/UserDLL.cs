using Microsoft.EntityFrameworkCore;
using Data;
using Data.EF;
using System.Diagnostics;

namespace DataLayer
{
    public class UserDLL
    {
        // ================ CRUD ================
        public static async Task<int> Add(User user)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                db.Users.Add(user);
                db.SaveChanges();
                return user.UserId;
            }
            catch (Exception ex)
            {
                WriteEventLog("Add User Error", ex);
                return 0;
            }
        }

        public static async Task<User?> GetByPhone(string phone)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(phone)) return null;
                using var db = new GamersPlatDbContext();
                return await db.Users
                    .Include(u => u.UserRoles)
                    .Include(u => u.RefreshTokens)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.PhoneNumber == phone);
            }
            catch (Exception ex)
            {
                WriteEventLog("Get User By Phone Error", ex);
                return null;
            }
        }

        public static async Task<bool> ChangePassword(int userId, string newPasswordHash)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = await db.Users.FirstOrDefaultAsync(u => u.UserId == userId);
                if (existing == null) return false;
                existing.PasswordHash = newPasswordHash;
                existing.UpdatedAt = DateTime.UtcNow;
                return db.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                WriteEventLog("Update Password Error", ex);
                return false;
            }
        }

        public static async Task<bool> VerifyEmail(int userId)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = await db.Users.FirstOrDefaultAsync(u => u.UserId == userId);
                if (existing == null) return false;
                existing.EmailVerified = true;
                existing.UpdatedAt = DateTime.UtcNow;
                return db.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                WriteEventLog("Verify Email Error", ex);
                return false;
            }
        }

        public static async Task<bool> VerifyPhone(int userId)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = await db.Users.FirstOrDefaultAsync(u => u.UserId == userId);
                if (existing == null) return false;
                existing.PhoneVerified = true;
                existing.UpdatedAt = DateTime.UtcNow;
                return db.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                WriteEventLog("Verify Phone Error", ex);
                return false;
            }
        }

        public static async Task<bool> Update(User user)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = await db.Users.FirstOrDefaultAsync(u => u.UserId == user.UserId);
                if (existing == null) return false;
                db.Entry(existing).CurrentValues.SetValues(user);
                return db.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                WriteEventLog("Update User Error", ex);
                return false;
            }
        }

        public static async Task<bool> Delete(int userId)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = await db.Users.FirstOrDefaultAsync(u => u.UserId == userId);
                if (existing == null) return false;
                db.Users.Remove(existing);
                return db.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                WriteEventLog("Delete User Error", ex);
                return false;
            }
        }

        public static async Task<User?> GetByID(int userId)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                return await db.Users
                    .Include(u => u.UserRoles)
                    .Include(u => u.RefreshTokens)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.UserId == userId);
            }
            catch (Exception ex)
            {
                WriteEventLog("Get User By ID Error", ex);
                return null;
            }
        }

        public static async Task<List<User>> GetAll()
        {
            try
            {
                using var db = new GamersPlatDbContext();
                return await db.Users
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                WriteEventLog("Get All Users Error", ex);
                return new List<User>();
            }
        }

        // ================ Validation ============
        public static async Task<bool> ExistsByEmail(string? email, int excludeId = 0)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email)) return false;
                using var db = new GamersPlatDbContext();
                return await db.Users.AnyAsync(u => u.Email == email && (excludeId == 0 || u.UserId != excludeId));
            }
            catch (Exception ex)
            {
                WriteEventLog("Exists By Email Error", ex);
                return false;
            }
        }

        public static async Task<bool> ExistsByPhone(string? phone, int excludeId = 0)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(phone)) return false;
                using var db = new GamersPlatDbContext();
                return await db.Users.AnyAsync(u => u.PhoneNumber == phone && (excludeId == 0 || u.UserId != excludeId));
            }
            catch (Exception ex)
            {
                WriteEventLog("Exists By Phone Error", ex);
                return false;
            }
        }

        // ===================== EventLog Helper =====================
        private static void WriteEventLog(string title, Exception ex)
        {
            string error = ex.Message;
            if (ex.InnerException != null)
                error += "\nInner Exception: " + ex.InnerException.Message;

            EventLog.WriteEntry("Application", $"{title}: {error}", EventLogEntryType.Error);
        }
    }
}
