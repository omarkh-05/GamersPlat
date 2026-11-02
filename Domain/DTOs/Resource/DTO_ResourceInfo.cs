using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Domain.DTOs.Resource
{
    public class DTO_ResourceInfo
    {
        public int ResourcesTypeId { get; set; }

        public int CenterId { get; set; }

        public byte DeviceId { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        public decimal HourlyPrice { get; set; }

        public int TotalQuantity { get; set; }

        [StringLength(10)]
        public string? RoomType { get; set; }

        public bool IsActive { get; set; }
    }
}
