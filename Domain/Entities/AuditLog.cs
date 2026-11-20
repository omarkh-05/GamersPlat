using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Data;

public partial class AuditLog
{
    [Key]
    public int AuditId { get; set; }

    public int? UserId { get; set; }

    [StringLength(100)]
    public string ActionDone { get; set; } = null!;

    [StringLength(100)]
    public string EntityName { get; set; } = null!;

    public int EntityId { get; set; }

    public string OldValues { get; set; } = null!;

    public string NewValues { get; set; } = null!;

    [Column("IPAddress")]
    [StringLength(45)]
    public string Ipaddress { get; set; } = null!;

    [StringLength(300)]
    public string UserAgent { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("AuditLogs")]
    public virtual User? User { get; set; }
}
