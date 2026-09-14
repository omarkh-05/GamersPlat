using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DTOs.User
{
    public class DTO_UpdateUserInfoRequest
    {
        public string FullName { get; set; } = null!;

        public string PhoneNumber { get; set; } = null!;

        public string PasswordHash { get; set; } = null!;

        public string? Email { get; set; }

        public short CityId { get; set; } = 0;
    }
}
