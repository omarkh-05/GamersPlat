using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Data;

[PrimaryKey("CenterId", "ServiceId")]
public partial class CenterService
{
    [Key]
    public int CenterId { get; set; }

    [Key]
    public byte ServiceId { get; set; }

    public bool IsActive { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("CenterId")]
    [InverseProperty("CenterServices")]
    public virtual Center Center { get; set; } = null!;

    [ForeignKey("ServiceId")]
    [InverseProperty("CenterServices")]
    public virtual Service Service { get; set; } = null!;
}
