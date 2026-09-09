using System;
using System.Collections.Generic;

namespace Data;

public partial class Booking
{
    public int BookingId { get; set; }

    public int? UserId { get; set; }

    public int CenterId { get; set; }

    public int ResourcesTypeId { get; set; }

    public string? GameName { get; set; }

    public DateOnly BookingDate { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal? Subtotal { get; set; }

    public string? DiscountType { get; set; }

    public double? DiscountValue { get; set; }

    public decimal TotalPrice { get; set; }

    public int? EarnedPoints { get; set; }

    public string Status { get; set; } = null!;

    public string CustomerName { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public int? OfferId { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? CancelledAt { get; set; }

    public string? CancellationReason { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Center Center { get; set; } = null!;

    public virtual Offer? Offer { get; set; }

    public virtual ResourcesType ResourcesType { get; set; } = null!;

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual User? User { get; set; }
}
