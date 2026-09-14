using Microsoft.EntityFrameworkCore;
using Data;
using Data.EF;
using System.Diagnostics;

namespace DataLayer
{
    public class AuditLogDLL
    {
        // ================ CRUD ================
        public static async Task<int> Add(AuditLog a)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                db.AuditLogs.Add(a);
                db.SaveChanges();
                return a.AuditId;
            }
            catch (Exception ex)
            {
                Data.DLL.EventLog_Helper.WriteEventLog("Add AuditLog Error", ex);
                return 0;
            }
        }
        // ================ CRUD ================


        // ================ Read By ================
        public static async Task<List<AuditLog>> GetByUserId(int userId)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                return await db.AuditLogs.Where(x => x.UserId == userId).AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                Data.DLL.EventLog_Helper.WriteEventLog("Get AuditLogs By User Error", ex);
                return new List<AuditLog>();
            }
        }
        // ================ Read By ================

    }
}
