using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Data;

public partial class ResourcesType
{
    [Key]
    public int ResourcesTypeId { get; set; }

    public int CenterId { get; set; }

    public byte DeviceId { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal HourlyPrice { get; set; }

    public int TotalQuantity { get; set; }

    [StringLength(10)]
    public string? RoomType { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    [StringLength(50)]
    public string? Unit { get; set; }

    [StringLength(500)]
    public string? ImageUrl { get; set; }

    public bool IsActive { get; set; }

    [InverseProperty("ResourcesType")]
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<ResourceUnit> ResourceUnits { get; set; } = new List<ResourceUnit>();

    [ForeignKey("CenterId")]
    [InverseProperty("ResourcesTypes")]
    public virtual Center Center { get; set; } = null!;

    [ForeignKey("DeviceId")]
    [InverseProperty("ResourcesTypes")]
    public virtual Device Device { get; set; } = null!;
}
