using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Domain.DTOs.Invoice
{
    public class DTO_CenterInvoices
    {
        public int InvoiceNumber { get; set; }
        public int BookingNumber { get; set; }
        public string CenterName { get; set; } = null!;
        public DateTime StartedAt { get; set; }
        public DateTime EndedAt { get; set; }
        public string ResourceBooked { get; set; } = null!;

        public decimal UnitPrice { get; set; }
        public decimal? Subtotal { get; set; }
        public decimal? DiscountValue { get; set; }
        public string? DiscountType { get; set; }
        public decimal TotalPrice { get; set; }

        public string AcceptedBy { get; set; }= null!;
        public string RejectedBy { get; set; }= null!;
        public string StartedBy { get; set; } = null!;
        public string StoppedBy { get; set; } = null!;
    }
}
