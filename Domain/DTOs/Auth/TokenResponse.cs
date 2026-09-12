using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DTOs.Auth
{
    public class TokenResponse
    {
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
    }
}
