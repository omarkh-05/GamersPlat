using System;
using System.Collections.Generic;

namespace Data;

public partial class AuditLog
{
    public int AuditId { get; set; }

    public int? UserId { get; set; }

    public string? ActionDone { get; set; }

    public string? EntityName { get; set; }

    public int? EntityId { get; set; }

    public string? OldValues { get; set; }

    public string? NewValues { get; set; }

    public string? Ipaddress { get; set; }

    public string? UserAgent { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User? User { get; set; }
}
