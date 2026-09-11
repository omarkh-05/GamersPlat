namespace GamersPlatAPI.DTOs
{
    public class OwnerDashboardDTO
    {
        public int OwnerUserId { get; set; }
        public int CentersCount { get; set; }
        public int BookingsCount { get; set; }
        public decimal RevenueTotal { get; set; }
        public int DevicesCount { get; set; }
        public int TournamentsCount { get; set; }
    }
}
