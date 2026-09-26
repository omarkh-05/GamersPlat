using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DTOs.Resource
{
    public class DTO_UpdateResource
    {
        public int CenterId { get; set; }

        public byte DeviceId { get; set; }

        public decimal HourlyPrice { get; set; }

        public int TotalQuantity { get; set; }

        public string? RoomType { get; set; }

        public bool IsActive { get; set; }
    }
}
