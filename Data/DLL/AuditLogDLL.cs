using Microsoft.EntityFrameworkCore;
using Data;
using Data.EF;
using System.Diagnostics;

namespace DataLayer
{
    public class AuditLogDLL
    {
        public static int Add(AuditLog a)
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
                WriteEventLog("Add AuditLog Error", ex);
                return 0;
            }
        }

        public static async Task<List<AuditLog>> GetByUserId(int userId)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                return await db.AuditLogs.Where(x => x.UserId == userId).AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                WriteEventLog("Get AuditLogs By User Error", ex);
                return new List<AuditLog>();
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
