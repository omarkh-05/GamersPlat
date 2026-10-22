using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DTOs.User
{
    public class DTO_UserInfoRequest
    {
        public int UserId { get; set; }
        public string? FullName { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Email { get; set; }

        public string CityName { get; set; } = "";

        public int Points { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        public ICollection<string>? Roles { get; set; } = new List<string>();

        public bool PhoneVerified { get; set; } = false;

        public bool EmailVerified { get; set; } = false;
    }
}
