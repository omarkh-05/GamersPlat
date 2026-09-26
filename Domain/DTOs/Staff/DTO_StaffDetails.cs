using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DTOs.Staff
{
    public class DTO_StaffDetails
    {
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string CenterName { get; set; } = string.Empty;
        public string StaffRoleName { get; set; } = string.Empty;
        public int Permissions { get; set; }
        public bool IsActive { get; set; }
    }
}
