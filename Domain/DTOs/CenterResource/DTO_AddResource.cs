using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DTOs.Resource
{
    public class DTO_AddResource
    {
        public byte ResourceId { get; set; }

        public decimal HourlyPrice { get; set; }

        public int TotalQuantity { get; set; }

        public string? RoomType { get; set; }

        public string? Description { get; set; }

        public bool IsActive { get; set; }
    }
}
