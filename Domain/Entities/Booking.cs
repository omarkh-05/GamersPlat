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

    public int UserId { get; set; }

    public int CenterId { get; set; }

    public int CenterResourceId { get; set; }

    public DateOnly BookingDate { get; set; }

    [StringLength(150)]
    public string CustomerName { get; set; } = null!;

    [StringLength(30)]
    public string PhoneNumber { get; set; } = null!;

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public int Quantity { get; set; }

    [StringLength(50)]
    public string? DiscountType { get; set; }

    public double? DiscountValue { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal UnitPrice { get; set; }

    [Column(TypeName = "decimal(7, 2)")]
    public decimal? Subtotal { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal TotalPrice { get; set; }

    public int? EarnedPoints { get; set; }

    [StringLength(50)]
    public string Status { get; set; } = null!;

    public int? OfferId { get; set; }

    [StringLength(300)]
    public string? GameName { get; set; }

    public bool IsApproved { get; set; }

    [Precision(0)]
    public DateTime? StartedAt { get; set; }

    [Precision(0)]
    public DateTime? EndedAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CancelledAt { get; set; }

    [StringLength(500)]
    public string? CancellationReason { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    public int? AcceptedBy { get; set; }
              
    public int? RejectedBy { get; set; }
              
    public int? StartedBy { get; set; }
              
    public int? StoppedBy { get; set; }


    [ForeignKey("CenterId")]
    [InverseProperty("Bookings")]
    public virtual Center Center { get; set; } = null!;

    [ForeignKey("CenterResourceId, CenterId")]
    [InverseProperty("Bookings")]
    public virtual CenterResource CenterResource { get; set; } = null!;

    [InverseProperty("Booking")]
    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    [ForeignKey("OfferId")]
    [InverseProperty("Bookings")]
    public virtual Offer? Offer { get; set; }

    [InverseProperty("Booking")]
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    [ForeignKey("AcceptedBy")]
    [InverseProperty("BookingAcceptedByNavigations")]
    public virtual User? AcceptedByNavigation { get; set; } = null!;
    [ForeignKey("RejectedBy")]
    [InverseProperty("BookingRejectedByNavigations")]
    public virtual User? RejectedByNavigation { get; set; } = null!;
    [ForeignKey("StartedBy")]
    [InverseProperty("BookingStartedByNavigations")]
    public virtual User? StartedByNavigation { get; set; } = null!;

    [ForeignKey("StoppedBy")]
    [InverseProperty("BookingStoppedByNavigations")]
    public virtual User? StoppedByNavigation { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("BookingUsers")]
    public virtual User User { get; set; } = null!;
}
