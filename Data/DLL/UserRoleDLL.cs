using Microsoft.EntityFrameworkCore;
using Data;
using Data.DLL;
using Data.EF;
using System.Diagnostics;

namespace DataLayer
{
    public class UserRoleDLL
    {
        // ================ CRUD ================
        public static async Task<int> Add(UserRole ur)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                db.UserRoles.Add(ur);
                await db.SaveChangesAsync();
                return ur.Id;
            }
            catch (Exception ex)
            {
               EventLog_Helper.WriteEventLog("Add UserRole Error", ex);
                return 0;
            }
        }
        // ================ CRUD ================


        // ================ Read By ================
        public static async Task<List<UserRole>> GetByUserId(int userId)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                return await db.UserRoles.Where(x => x.UserId == userId).AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog("Get UserRoles By User Error", ex);
                return new List<UserRole>();
            }
        }
        // ================ Read By ================
    }
}
