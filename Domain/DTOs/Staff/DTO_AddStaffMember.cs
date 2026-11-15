using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Domain.DTOs.Staff
{
    public class DTO_AddStaffMember
    {
        public string FullName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string? Email { get; set; }
        public short CityId { get; set; }
        public bool IsActive { get; set; }
        public string Status { get; set; } = "active";

        public int CenterId { get; set; }
        public short Permissions { get; set; }
        public string? RoleName { get; set; }
    }
}
