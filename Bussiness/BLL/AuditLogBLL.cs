using Data;
using DataLayer;
namespace Bussiness
{
    public class AuditLogBLL
    {
        // ================ CRUD ================
        public async Task<bool> Add(AuditLog auditLog)
        {
           int _auditLogID = await AuditLogDLL.Add(auditLog);
            return _auditLogID > 0;
        }
        // ================ CRUD ================


        // ================ Read By ================
        public async Task<List<AuditLog>> GetByUserId(int userId) => await AuditLogDLL.GetByUserId(userId);
        // ================ Read By ================
    }
}
