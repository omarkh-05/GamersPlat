namespace Domain.DTOs.Invoice
{
    public class DTO_EndBookingInvoice
    {
        public bool UseOriginalPrice { get; set; }
        public string? DiscountType { get; set; }
        public decimal? DiscountValue { get; set; }
    }
}
