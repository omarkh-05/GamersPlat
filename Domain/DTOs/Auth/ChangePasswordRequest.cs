using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DTOs.Auth
{
    public class ChangePasswordRequest
    {
        public string NewPassword { get; set; } = null!;
        public string CurrentPassword { get; set; } = null!;
    }
}
