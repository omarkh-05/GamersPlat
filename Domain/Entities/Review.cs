using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Data;

[Index("UserId", "BookingId", Name = "UQ_Reviews_UserId_BookingId", IsUnique = true)]
public partial class Review
{
    [Key]
    public int ReviewId { get; set; }

    public int UserId { get; set; }

    public int CenterId { get; set; }

    public int BookingId { get; set; }

    public int Rating { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("BookingId")]
    [InverseProperty("Reviews")]
    public virtual Booking Booking { get; set; } = null!;

    [ForeignKey("CenterId")]
    [InverseProperty("Reviews")]
    public virtual Center Center { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("Reviews")]
    public virtual User User { get; set; } = null!;
}
