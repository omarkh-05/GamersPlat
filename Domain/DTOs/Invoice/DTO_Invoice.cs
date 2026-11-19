using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Domain.DTOs.Invoice
{
    public class DTO_Invoice
    {
        public int InvoiceNumber { get; set; }
        public int BookingNumber { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime EndedAt { get; set; }
        public string ResourceBooked { get; set; } = null!;
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? DiscountValue { get; set; }
        [StringLength(50)]
        public string? DiscountType { get; set; }
        [Column(TypeName = "decimal(5, 2)")]
        public decimal UnitPrice { get; set; }
        [Column(TypeName = "decimal(7, 2)")]
        public decimal? Subtotal { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal TotalPrice { get; set; }
    }
}
