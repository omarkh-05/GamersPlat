using Data;
using DataLayer;
using Bussiness.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Bussiness
{
    public class AuditLogBLL : IAuditLogService
    {
        private readonly AuditLogDLL _auditLogDLL;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditLogBLL(AuditLogDLL auditLogDLL, IHttpContextAccessor httpContextAccessor)
        {
            _auditLogDLL = auditLogDLL;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task LogAsync(
            int? userId,
            string actionDone,
            string entityName,
            int entityId,
            object? oldValues = null,
            object? newValues = null)
        {
            try
            {
                var request = _httpContextAccessor.HttpContext?.Request;
                var auditLog = new AuditLog
                {
                    UserId = userId,
                    ActionDone = Limit(actionDone, 100),
                    EntityName = Limit(entityName, 100),
                    EntityId = entityId,
                    OldValues = JsonSerializer.Serialize(oldValues),
                    NewValues = JsonSerializer.Serialize(newValues),
                    Ipaddress = Limit(request?.HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty, 45),
                    UserAgent = Limit(request?.Headers.UserAgent.ToString() ?? string.Empty, 300),
                    CreatedAt = DateTime.UtcNow
                };

                await _auditLogDLL.Add(auditLog);
            }
            catch (Exception exception)
            {
                System.Diagnostics.Trace.TraceError("Audit logging failed: {0}", exception);
            }
        }

        private static string Limit(string value, int maxLength)
            => value.Length <= maxLength ? value : value[..maxLength];

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