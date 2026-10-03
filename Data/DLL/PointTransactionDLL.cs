using Data;
using Data.DLL;
using Data.EF;
using Microsoft.EntityFrameworkCore;

namespace DataLayer
{
    public class PointTransactionDLL
    {
        private readonly GamersPlatDbContext _db;

        public PointTransactionDLL(GamersPlatDbContext db)
        {
            _db = db;
        }

        // ================ CRUD ================
        public async Task<int> Add(PointTransaction pt)
        {
            try
            {
                _db.PointTransactions.Add(pt);
                await _db.SaveChangesAsync();

                return pt.TransactionId;
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog(
                    "Add PointTransaction Error",
                    ex);

                return 0;
            }
        }
        // ================ CRUD ================


        // ================ Read By ================
        public async Task<List<PointTransaction>> GetByUserId(int userId)
        {
            try
            {
                return await _db.PointTransactions
                    .Where(p => p.UserId == userId)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                EventLog_Helper.WriteEventLog(
                    "Get PointTransactions By User Error",
                    ex);

                return new List<PointTransaction>();
            }
        }
        // ================ Read By ================
    }
}