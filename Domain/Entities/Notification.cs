using System;
using System.Collections.Generic;

namespace Data;

public partial class Notification
{
    public int NotificationId { get; set; }

    public int UserId { get; set; }

    public int? CenterId { get; set; }

    public string Title { get; set; } = null!;

    public string Message { get; set; } = null!;

    public string Type { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string ReferenceType { get; set; } = null!;

    public DateTime? ReadAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Center? Center { get; set; }

    public virtual User User { get; set; } = null!;
}
