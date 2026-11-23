using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DTOs.Notification
{
    public class DTO_BookingNotification
    {
        public int BookingId { get; set; }
        public int CenterId { get; set; }
        public int UserId { get; set; }
        public int OwnerUserId { get; set; }

        public string PlayerName { get; set; }
        public string CenterName { get; set; }

        public DateOnly BookingDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public decimal TotalPrice { get; set; }

        public string? AcceptedByName { get; set; }
        public string? RejectedByName { get; set; }
    }
}
