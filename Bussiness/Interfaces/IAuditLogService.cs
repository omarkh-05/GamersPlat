namespace Bussiness.Interfaces
{
    public interface IAuditLogService
    {
        Task LogAsync(
            int? userId,
            string actionDone,
            string entityName,
            int entityId,
            object? oldValues = null,
            object? newValues = null);
    }
}
