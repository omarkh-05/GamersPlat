using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DTOs.Staff
{
    public class DTO_StaffDetails
    {
        public int StaffId { get; set; }
        public int CenterId { get; set; }
        public string FullName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string CenterName { get; set; } = null!;
        public string StaffRoleName { get; set; } = null!;
        public int Permissions { get; set; }
        public string Status { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}
