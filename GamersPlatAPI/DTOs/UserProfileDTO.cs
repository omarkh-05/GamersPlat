namespace GamersPlatAPI.DTOs
{
    public class UserProfileDTO
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = null!;
        public string? Email { get; set; }
        public string PhoneNumber { get; set; } = null!;
        public int Points { get; set; }
        public bool IsActive { get; set; }
        public short CityId { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool EmailVerified { get; set; }
        public bool PhoneVerified { get; set; }
    }
}
