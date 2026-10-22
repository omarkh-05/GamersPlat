using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Domain.DTOs.User
{
    public class DTO_UpdateUserInfoRequest
    {
        [Required, StringLength(100, MinimumLength = 2)]
        public string FullName { get; set; } = null!;

        [Required, StringLength(20, MinimumLength = 7)]
        public string PhoneNumber { get; set; } = null!;

        [EmailAddress, StringLength(150)]
        public string? Email { get; set; }

        public short CityId { get; set; }
    }
}
