using System;

namespace Domain.DTOs.Invoice
{
    public class DTO_InvoicePreview
    {
        public int BookingNumber { get; set; }
        public DateOnly BookingDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime PreviewedAt { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal ActualPlayingTimePrice { get; set; }
        public decimal OriginalBookingPrice { get; set; }
        public string? DiscountType { get; set; }
        public double? DiscountValue { get; set; }
    }
}
