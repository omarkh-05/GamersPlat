using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DTOs.Booking
{
    public class DTO_BookingDetails
    {
        public string CustomerName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string? CenterName{ get; set; }
        public string? ResourcesType { get; set; }
        public DateOnly BookingDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public string EndTime { get; set; } = "";
        public string Duration { get; set; } = "";
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = null!;
    }
}
