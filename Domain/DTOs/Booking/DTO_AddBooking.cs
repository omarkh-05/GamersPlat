using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Domain.DTOs.Booking
{
    public class DTO_AddBooking
    {
        public int? UserId { get; set; }
        public int CenterId { get; set; }
        public int ResourcesTypeId { get; set; }

        public DateOnly BookingDate { get; set; }

        public TimeOnly StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }

        // Number of resources to book (default 1)
        public int Quantity { get; set; } = 1;

        public int? OfferId { get; set; }

        public string CustomerName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
    }
}
