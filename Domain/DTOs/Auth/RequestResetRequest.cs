using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DTOs.Auth
{
    public class RequestResetRequest
    {
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
