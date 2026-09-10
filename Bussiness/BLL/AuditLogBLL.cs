using Data;
using DataLayer;
namespace Bussiness
{
    public class AuditLogBLL
    {
        private enum enMode { AddMode = 1, UpdateMode = 2 }
        private enMode _mode = enMode.AddMode;

        private AuditLog _auditLog;
        public int _auditLogID = -1;

        public AuditLogBLL()
        {
            _auditLog = new AuditLog();
            _mode = enMode.AddMode;
        }

        public AuditLogBLL(AuditLog auditLog)
        {
            _auditLog = auditLog;
            _mode = enMode.UpdateMode;
        }

        public AuditLog CurrentAuditLog { get => _auditLog; set => _auditLog = value; }

        public bool Add()
        {
            _auditLogID = AuditLogDLL.Add(_auditLog);
            return _auditLogID > 0;
        }

        public static async Task<List<AuditLog>> GetByUserId(int userId) => await AuditLogDLL.GetByUserId(userId);

        public bool Save() => _mode switch
        {
            enMode.AddMode => Add(),
            _ => false
        };
    }
}
