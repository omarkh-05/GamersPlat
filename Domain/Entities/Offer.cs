using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Data;

public partial class Offer
{
    [Key]
    public int OfferId { get; set; }

    public int CenterId { get; set; }

    [StringLength(150)]
    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    [StringLength(50)]
    public string? DiscountType { get; set; }

    public double? DiscountValue { get; set; }

    public short? MaxUses { get; set; }

    public short? CurrentUses { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime StartDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime EndDate { get; set; }

    public TimeOnly? StartTime { get; set; }

    public TimeOnly? EndTime { get; set; }

    public bool IsActive { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [InverseProperty("Offer")]
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    [ForeignKey("CenterId")]
    [InverseProperty("Offers")]
    public virtual Center Center { get; set; } = null!;
}
