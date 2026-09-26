using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Data;

public partial class Notification
{
    [Key]
    public int NotificationId { get; set; }

    public int UserId { get; set; }

    public int? CenterId { get; set; }

    [StringLength(150)]
    public string Title { get; set; } = null!;

    public string Message { get; set; } = null!;

    [StringLength(50)]
    public string Type { get; set; } = null!;

    [StringLength(50)]
    public string Status { get; set; } = null!;

    [StringLength(50)]
    public string ReferenceType { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? ReadAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("CenterId")]
    [InverseProperty("Notifications")]
    public virtual Center? Center { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Notifications")]
    public virtual User User { get; set; } = null!;
}
