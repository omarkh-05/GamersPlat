using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DTOs.Center
{
    public class DTO_CentersListDetails
    {
        public string CenterName { get; set; } = null!;
        public string CityName { get; set; } = null!;
        public string CenterAddress { get; set; } = null!;
        public string CenterDescription { get; set; } = null!;
        public string CenterStatus { get; set; } = null!;
        public string CenterType { get; set; } = null!;
        public TimeOnly? OpenTime { get; set; }
        public TimeOnly? CloseTime { get; set; }
        public decimal Rating { get; set; }
        public decimal ResourcesCount { get; set; }
        public List<string> ServicesList { get; set; } = new List<string>();
    }
}
