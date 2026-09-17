using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DTOs.Center
{
    public class DTO_UpdateCenter
    {
        public int CenterId { get; set; }
        public string CenterName { get; set; } = null!;
        public short CityId { get; set; }
        public string CenterAddress { get; set; } = null!;
        public string CenterDescription { get; set; } = null!;
        public string CenterStatus { get; set; } = null!;
        public bool IsActive { get; set; }
        public string CenterType { get; set; } = null!;
        public TimeOnly OpenTime { get; set; }
        public TimeOnly CloseTime { get; set; }
    }
}
