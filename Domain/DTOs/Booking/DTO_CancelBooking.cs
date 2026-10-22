using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DTOs.Booking
{
    public class DTO_CancelBooking
    {
        public int BookingId { get; set; }
        public string? CancelReason { get; set; }
    }
}
