using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Data;

[Index("CenterId", "DeviceId", Name = "UQ__DeviceTy__F8D8EF08BA9EF532", IsUnique = true)]
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

    public bool IsActive { get; set; }

    [InverseProperty("ResourcesType")]
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    [ForeignKey("CenterId")]
    [InverseProperty("ResourcesTypes")]
    public virtual Center Center { get; set; } = null!;

    [ForeignKey("DeviceId")]
    [InverseProperty("ResourcesTypes")]
    public virtual Device Device { get; set; } = null!;
}
