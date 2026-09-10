using Microsoft.EntityFrameworkCore;
using Data;
using Data.EF;
using System.Diagnostics;

namespace DataLayer
{
    public class PointTransactionDLL
    {
        public static int Add(PointTransaction pt)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                db.PointTransactions.Add(pt);
                db.SaveChanges();
                return pt.TransactionId;
            }
            catch (Exception ex)
            {
                WriteEventLog("Add PointTransaction Error", ex);
                return 0;
            }
        }

        public static async Task<List<PointTransaction>> GetByUserId(int userId)
        {
            try
            {
                using var db = new GamersPlatDbContext();
                return await db.PointTransactions.Where(p => p.UserId == userId).AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                WriteEventLog("Get PointTransactions By User Error", ex);
                return new List<PointTransaction>();
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
