using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DTOs.User
{
    public class DTO_HomeCurrnetUserInfo
    {
        public string FullName { get; set; } = "";
        public IEnumerable<string>? Role { get; set; } = new List<string>();
    }
}
