using System;
using System.Collections.Generic;

namespace Data;

public partial class Review
{
    public int ReviewId { get; set; }

    public int UserId { get; set; }

    public int CenterId { get; set; }

    public int BookingId { get; set; }

    public int Rating { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Booking Booking { get; set; } = null!;

    public virtual Center Center { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
