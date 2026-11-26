using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DTOs.Auth
{
    public class RequestResetRequest
    {
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
    }
}
