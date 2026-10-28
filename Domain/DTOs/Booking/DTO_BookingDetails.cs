using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DTOs.Booking
{
    public class DTO_BookingDetails
    {
        public int BookingId { get; set; }
        public int CenterId { get; set; }
        public int? PlayerId { get; set; }
        public string CustomerName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string? CenterName{ get; set; }
        public string? ResourcesType { get; set; }
        public DateOnly BookingDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly? EndTime { get; set; } = TimeOnly.MinValue;
        public string? Duration { get; set; } = "";
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = null!;
        public string? Notes { get; set; }
    }
}
