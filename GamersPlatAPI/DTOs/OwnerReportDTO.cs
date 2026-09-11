namespace GamersPlatAPI.DTOs
{
    public class OwnerReportDTO
    {
        public int OwnerUserId { get; set; }
        public int CentersCount { get; set; }
        public int BookingsCount { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalEarnedPoints { get; set; }
    }
}
