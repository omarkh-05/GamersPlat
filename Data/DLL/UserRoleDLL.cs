using Microsoft.EntityFrameworkCore;
using Data;
using Data.EF;
using System.Diagnostics;

namespace DataLayer
{
    public class UserRoleDLL
    {
        public static int Add(UserRole ur)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                db.UserRoles.Add(ur);
                db.SaveChanges();
                return ur.Id;
            }
            catch (Exception ex)
            {
                WriteEventLog("Add UserRole Error", ex);
                return 0;
            }
        }

        public static async Task<List<UserRole>> GetByUserId(int userId)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                return await db.UserRoles.Where(x => x.UserId == userId).AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                WriteEventLog("Get UserRoles By User Error", ex);
                return new List<UserRole>();
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
