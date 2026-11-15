using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DTOs.Service
{
    public class DTO_AddCenterService
    {
        public byte ServiceId { get; set; }

        public string Icon { get; set; }

        public bool IsActive { get; set; }
    }
}
