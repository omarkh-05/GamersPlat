using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DTOs.Auth
{
    public class RefreshRequest
    {
        public string PhoneNumber { get; set; } = null!;
    }
}
