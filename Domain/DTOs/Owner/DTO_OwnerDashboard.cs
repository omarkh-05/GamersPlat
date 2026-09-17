using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DTOs.Owner
{
    public class DTO_OwnerDashboard
    {
        public string FullName { get; set; } = string.Empty;
        public short TotalCenters { get; set; }
        public short ActiveCenters { get; set; }

        public int TotalBookings { get; set; }
        public int UpcomingBookings { get; set; }
        public int CompletedBookings { get; set; }
        public int CancelledBookings { get; set; }

        public decimal TotalRevenue { get; set; }

        public decimal AverageRating { get; set; }
        public int TotalReviews { get; set; }

        public short ActiveServices { get; set; }
        public short ActiveResources { get; set; }
        public short ActiveOffers { get; set; }
        public short ActiveTournaments { get; set; }
    }
}