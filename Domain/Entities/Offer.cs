using System;
using System.Collections.Generic;

namespace Data;

public partial class Offer
{
    public int OfferId { get; set; }

    public int CenterId { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string? DiscountType { get; set; }

    public double? DiscountValue { get; set; }

    public short? MaxUses { get; set; }

    public short? CurrentUses { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public TimeOnly? StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public bool IsActive { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Center Center { get; set; } = null!;
}
