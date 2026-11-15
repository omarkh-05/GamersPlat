using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DTOs.Service
{
    public class DTO_UpdateCenterService
    {
        public byte ServiceId { get; set; }
        public string Icon { get; set; } = "other";
        public bool IsActive { get; set; }
    }
}
