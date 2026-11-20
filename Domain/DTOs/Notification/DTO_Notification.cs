namespace Domain.DTOs.Notification;

public sealed class DTO_Notification
{
    public int NotificationId { get; set; }
    public int? CenterId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string ReferenceType { get; set; } = string.Empty;
    public DateTime? ReadAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
