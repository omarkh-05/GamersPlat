using Data;
using DataLayer;
namespace Bussiness
{
    public class AuditLogBLL
    {
        // ================ CRUD ================
        public bool Add(AuditLog auditLog)
        {
           int _auditLogID = AuditLogDLL.Add(auditLog);
            return _auditLogID > 0;
        }
        // ================ CRUD ================


        // ================ Read By ================
        public static async Task<List<AuditLog>> GetByUserId(int userId) => await AuditLogDLL.GetByUserId(userId);
        // ================ Read By ================
    }
}
