using Microsoft.EntityFrameworkCore;
using Data;
using Data.EF;
using System.Diagnostics;

namespace DataLayer
{
    public class UserDLL
    {
        // ================ CRUD ================
        public static int Add(User user)
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

        public static bool Update(User user)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = db.Users.FirstOrDefault(u => u.UserId == user.UserId);
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

        public static bool Delete(int userId)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                var existing = db.Users.FirstOrDefault(u => u.UserId == userId);
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
        public static bool ExistsByEmail(string? email, int excludeId = 0)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email)) return false;
                using var db = new GamersPlatDbContext();
                return db.Users.Any(u => u.Email == email && (excludeId == 0 || u.UserId != excludeId));
            }
            catch (Exception ex)
            {
                WriteEventLog("Exists By Email Error", ex);
                return false;
            }
        }

        public static bool ExistsByPhone(string? phone, int excludeId = 0)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(phone)) return false;
                using var db = new GamersPlatDbContext();
                return db.Users.Any(u => u.PhoneNumber == phone && (excludeId == 0 || u.UserId != excludeId));
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
