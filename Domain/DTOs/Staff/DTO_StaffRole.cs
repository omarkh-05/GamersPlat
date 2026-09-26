using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DTOs.Staff
{
    public class DTO_StaffRole
    {
        public int CenterId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Permissions { get; set; }
    }
}
