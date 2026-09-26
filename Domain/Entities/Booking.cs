using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Data;

public partial class Booking
{
    [Key]
    public int BookingId { get; set; }

    public int? UserId { get; set; }

    public int CenterId { get; set; }

    public int ResourcesTypeId { get; set; }

    [StringLength(300)]
    public string? GameName { get; set; }

    public DateOnly BookingDate { get; set; }

    public int Quantity { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal UnitPrice { get; set; }

    [Column(TypeName = "decimal(7, 2)")]
    public decimal? Subtotal { get; set; }

    [StringLength(50)]
    public string? DiscountType { get; set; }

    public double? DiscountValue { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal TotalPrice { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public int? EarnedPoints { get; set; }

    [StringLength(50)]
    public string Status { get; set; } = null!;

    [StringLength(150)]
    public string CustomerName { get; set; } = null!;

    [StringLength(30)]
    public string PhoneNumber { get; set; } = null!;

    public int? OfferId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CancelledAt { get; set; }

    [StringLength(500)]
    public string? CancellationReason { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("CenterId")]
    [InverseProperty("Bookings")]
    public virtual Center Center { get; set; } = null!;

    [ForeignKey("OfferId")]
    [InverseProperty("Bookings")]
    public virtual Offer? Offer { get; set; }

    [ForeignKey("ResourcesTypeId")]
    [InverseProperty("Bookings")]
    public virtual ResourcesType ResourcesType { get; set; } = null!;

    [InverseProperty("Booking")]
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    [ForeignKey("UserId")]
    [InverseProperty("Bookings")]
    public virtual User? User { get; set; }
}
