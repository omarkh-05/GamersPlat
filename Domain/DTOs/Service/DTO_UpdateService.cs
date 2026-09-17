using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DTOs.Service
{
    public class DTO_UpdateService
    {
        public byte ServiceId { get; set; }

        public int CenterId { get; set; }

        public string Name { get; set; } = null!;

        public bool IsActive { get; set; }
    }
}
