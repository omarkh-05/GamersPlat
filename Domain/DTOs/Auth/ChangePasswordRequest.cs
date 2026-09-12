using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DTOs.Auth
{
    public class ChangePasswordRequest
    {
        public string PhoneNumber { get; set; } = null!;
        public string OldPassword { get; set; } = null!;
    }
}
