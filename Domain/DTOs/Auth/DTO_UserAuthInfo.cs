using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DTOs.Auth
{
    // DTO used only for authentication or password operations where PasswordHash and UserId are required
    public class DTO_UserAuthInfo
    {
        public int UserId { get; set; }
        public string PasswordHash { get; set; } = null!;
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public bool IsActive { get; set; }
    }
}
