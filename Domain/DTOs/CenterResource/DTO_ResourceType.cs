using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DTOs.Resource
{
    public class DTO_ResourceType
    {
        public int CenterResourceId { get; set; }
        public int ResourcesId { get; set; }
        public string DeviceName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int AvailableQuantity { get; set; }
        public decimal HourlyPrice { get; set; }
    }
}
