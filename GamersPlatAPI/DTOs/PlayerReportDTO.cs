namespace GamersPlatAPI.DTOs
{
    public class PlayerReportDTO
    {
        public int UserId { get; set; }
        public int BookingsCount { get; set; }
        public decimal TotalSpent { get; set; }
        public int TotalPointsEarned { get; set; }
    }
}
