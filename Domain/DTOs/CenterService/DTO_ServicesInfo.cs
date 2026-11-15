using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Domain.DTOs.Service
{
    public class DTO_ServicesInfo
    {
        public int CenterServiceId { get; set; }
        public byte ServiceId { get; set; }
        public int CenterId { get; set; }
        public string Name { get; set; } = null!;
        public string Icon { get; set; } = "other";
        public bool IsActive { get; set; }
    }
}
