using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DTOs.Auth
{
    public class LoginRequest
    {
        public string PhoneNumber { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
