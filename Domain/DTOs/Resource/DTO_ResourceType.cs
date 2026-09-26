using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DTOs.Resource
{
    public class DTO_ResourceType
    {
        public int ResourcesTypeId { get; set; }
        public string DeviceName { get; set; } = string.Empty; // "PS5 Pro", "RTX 4080 PC"
        public string Category { get; set; } = string.Empty;   // "Console" | "PC"
        public int AvailableQuantity { get; set; }
        public decimal HourlyPrice { get; set; }
    }
}
