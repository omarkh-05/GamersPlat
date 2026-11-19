using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Data;

public partial class Invoice
{
    [Key]
    public int InvoiceId { get; set; }

    public int BookingId { get; set; }

    [Precision(0)]
    public DateTime StartedAt { get; set; }

    [Precision(0)]
    public DateTime EndedAt { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? DiscountValue { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal TotalPrice { get; set; }

    [StringLength(50)]
    public string? DiscountType { get; set; }

    [Column(TypeName = "decimal(5, 2)")]
    public decimal UnitPrice { get; set; }

    [Column(TypeName = "decimal(7, 2)")]
    public decimal? Subtotal { get; set; }

    [Precision(0)]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("BookingId")]
    [InverseProperty("Invoices")]
    public virtual Booking Booking { get; set; } = null!;
}
