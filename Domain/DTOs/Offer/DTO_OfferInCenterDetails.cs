using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DTOs.Offer
{
    public class DTO_OfferInCenterDetails
    {
        public int OfferId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string DiscountType { get; set; } = string.Empty; // "Percentage" | "Fixed" | "Bundle"
        public double? DiscountValue { get; set; }
        public decimal? Price { get; set; } // for bundle-style offers like "Weekend Pass $120"
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
