using Data;
using Data.DLL;
using Data.EF;
using Microsoft.EntityFrameworkCore;
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
                EventLog_Helper.WriteEventLog("Add PointTransaction Error", ex);
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
                EventLog_Helper.WriteEventLog("Get PointTransactions By User Error", ex);
                return new List<PointTransaction>();
            }
        }


    }
}
