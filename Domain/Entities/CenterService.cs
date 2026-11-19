using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Data;

[Index("CenterId", "ServiceId", Name = "UQ_CenterServices_Center_Service", IsUnique = true)]
public partial class CenterService
{
    [Key]
    public int CenterServiceId { get; set; }

    public int CenterId { get; set; }

    public byte ServiceId { get; set; }

    [StringLength(50)]
    public string Icon { get; set; } = null!;

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
