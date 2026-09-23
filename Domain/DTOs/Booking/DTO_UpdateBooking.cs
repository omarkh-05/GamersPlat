using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DTOs.Booking
{
    public class DTO_UpdateBooking
    {
        public int BookingId { get; set; }
        public DateOnly BookingDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
        public int Quantity { get; set; } = 1;
        public string CustomerName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
    }
}
