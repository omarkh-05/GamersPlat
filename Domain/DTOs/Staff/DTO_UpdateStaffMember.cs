using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DTOs.Staff
{
    public class DTO_UpdateStaffMember
    {
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string CenterName { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public string StaffRoleName { get; set; } = string.Empty;
        public int centerId { get; set; }
        public int Permissions { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
