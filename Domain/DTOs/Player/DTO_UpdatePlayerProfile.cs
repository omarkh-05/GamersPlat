
using System.ComponentModel.DataAnnotations;

namespace Domain.DTOs.Player
{
    public class DTO_UpdatePlayerProfile
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [EmailAddress]
        [StringLength(256)]
        public string? Email { get; set; }

        [Required]
        [StringLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Range((short)1, short.MaxValue)]
        public short CityId { get; set; }
    }
}
