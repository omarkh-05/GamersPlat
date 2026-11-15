using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Domain.DTOs.Resource
{
    public class DTO_ResourceInfo
    {
        public int CenterResourceId { get; set; }
        public int CenterId { get; set; }
        public int ResourcesTypeId { get; set; }
        public string ResourceName { get; set; } = string.Empty;

        [Column(TypeName = "decimal(10, 2)")]
        public decimal HourlyPrice { get; set; }

        public int TotalQuantity { get; set; }

        [StringLength(10)]
        public string? RoomType { get; set; }

        public string? Description { get; set; }

        public bool IsActive { get; set; }
    }
}
