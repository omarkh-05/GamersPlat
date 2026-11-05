using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Data;

[Index("CenterResourceId", "CenterId", Name = "UX_CenterResources_Id_Center", IsUnique = true)]
public partial class CenterResource
{
    [Key]
    public int CenterResourceId { get; set; }

    public int CenterId { get; set; }

    public int ResourcesTypeId { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal HourlyPrice { get; set; }

    public int TotalQuantity { get; set; }

    [StringLength(150)]
    public string? Description { get; set; }

    [StringLength(50)]
    public string RoomType { get; set; } = null!;

    public bool IsActive { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [InverseProperty("CenterResource")]
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    [ForeignKey("CenterId")]
    [InverseProperty("CenterResources")]
    public virtual Center Center { get; set; } = null!;

    [ForeignKey("ResourcesTypeId")]
    [InverseProperty("CenterResources")]
    public virtual Resource ResourcesType { get; set; } = null!;
}
