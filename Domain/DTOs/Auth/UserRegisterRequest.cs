using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Domain.DTOs.Auth
{
    public class UserRegisterRequest
    {
        [Required, MaxLength(100)]
        public string FullName { get; set; } = null!;

        [Required, MaxLength(20)]
        public string PhoneNumber { get; set; } = null!;

        [MaxLength(100)]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public short CityId { get; set; } = 1;

        [Required, MinLength(8)]
        public string Password { get; set; } = null!;

        public string? Role { get; set; } = "player";
    }
}
