namespace Domain.DTOs.Admin
{
    public class DTO_AdminStatistics
    {
        // Users
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int NewRegistrations { get; set; }
        public int BlockedUsers { get; set; }

        // Centers
        public int TotalCenters { get; set; }
        public int ActiveCenters { get; set; }
        public int PendingCenters { get; set; }
        public DTO_TopRatedCenter? TopRatedCenter { get; set; }

        // Top Performing Centers
        public List<DTO_TopPerformingCenter> TopPerformingCenters { get; set; } = new();

        // Platform Activity
        public int TotalBookings { get; set; }
        public int TotalSessions { get; set; }

        public string? PopularDevice { get; set; }
        public int PopularDevicePercentage { get; set; }

        public string? PopularLocation { get; set; }
        public int PopularLocationPercentage { get; set; }

        // Financial
        public decimal TotalRevenue { get; set; }
        public decimal RevenueGrowth { get; set; }
    }

    public class DTO_TopRatedCenter
    {
        public int CenterId { get; set; }
        public string CenterName { get; set; } = string.Empty;
        public decimal Rating { get; set; }
    }


    public class DTO_TopPerformingCenter
    {
        public int CenterId { get; set; }
        public string CenterName { get; set; } = string.Empty;
        public string CityName { get; set; } = string.Empty;
        public string OwnerName { get; set; } = string.Empty;

        public int Bookings { get; set; }
        public decimal Revenue { get; set; }
        public bool IsActive { get; set; }
    }
}
    
