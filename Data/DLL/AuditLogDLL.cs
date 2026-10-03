using Microsoft.EntityFrameworkCore;
using Data;
using Data.EF;
using System.Diagnostics;

namespace DataLayer
{
    public class AuditLogDLL
    {
        private readonly GamersPlatDbContext _db;

        public AuditLogDLL(GamersPlatDbContext db)
        {
            _db = db;
        }

        // ================ CRUD ================
        public async Task<int> Add(AuditLog a)
        {
            try
            {
                _db.AuditLogs.Add(a);
                await _db.SaveChangesAsync();
                return a.AuditId;
            }
            catch (Exception ex)
            {
                Data.DLL.EventLog_Helper.WriteEventLog(
                    "Add AuditLog Error",
                    ex);

                return 0;
            }
        }
        // ================ CRUD ================


        // ================ Read By ================
        public async Task<List<AuditLog>> GetByUserId(int userId)
        {
            try
            {
                return await _db.AuditLogs
                    .Where(x => x.UserId == userId)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Data.DLL.EventLog_Helper.WriteEventLog(
                    "Get AuditLogs By User Error",
                    ex);

                return new List<AuditLog>();
            }
        }
        // ================ Read By ================
    }
}