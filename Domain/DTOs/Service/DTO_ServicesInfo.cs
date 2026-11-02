using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Domain.DTOs.Service
{
    public class DTO_ServicesInfo
    {
        [Key]
        public byte ServiceId { get; set; }
        public int CenterId { get; set; }
        [StringLength(50)]
        public string Name { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}
