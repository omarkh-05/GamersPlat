using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DTOs.Staff
{
    public class DTO_CreateRole
    {
        public int CenterId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Permissions { get; set; }
    }
}
