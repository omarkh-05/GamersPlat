using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DTOs.User
{
    public class DTO_UserListResponse
    {
        public int UserId { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public int? CityId { get; set; }
        public int Points { get; set; }
        public bool IsActive { get; set; }
        public bool PhoneVerified { get; set; }
        public bool EmailVerified { get; set; }

        public List<string> Roles { get; set; } = new();
    }
}
