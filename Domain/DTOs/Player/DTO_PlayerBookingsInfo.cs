using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DTOs.Player
{
    public class DTO_PlayerBookingsInfo
    {
        public string CenterName { get; set; } = null!;
        public string Resource { get; set; } = null!;
        public string? GameName { get; set; } = "";
        public int? OfferId { get; set; } = 0;
        public int Quantity { get; set; } = 0;
        public decimal TotalPrice { get; set; } = 0;
        public int? EarnedPoints { get; set; } = 0;
        public string Status { get; set; } = null!;
        public DateOnly BookingDate { get; set; }
    }
}
