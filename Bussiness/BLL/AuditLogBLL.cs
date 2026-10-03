using Data;
using DataLayer;

namespace Bussiness
{
    public class AuditLogBLL
    {
        private readonly AuditLogDLL _auditLogDLL;

        public AuditLogBLL(AuditLogDLL auditLogDLL)
        {
            _auditLogDLL = auditLogDLL;
        }

        // ================ CRUD ================
        public async Task<bool> Add(AuditLog auditLog)
        {
            int auditLogID = await _auditLogDLL.Add(auditLog);
            return auditLogID > 0;
        }
        // ================ CRUD ================


        // ================ Read By ================
        public async Task<List<AuditLog>> GetByUserId(int userId)
            => await _auditLogDLL.GetByUserId(userId);
        // ================ Read By ================
    }
}