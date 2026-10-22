using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DTOs.Center
{
    public class DTO_HomePageCentersDetails
    {
        public int CenterId { get; set; } = 0;
        public string CenterName { get; set; } = null!;
        public string CityName { get; set; } = null!;
        public string CenterAddress { get; set; } = null!;
        public string CenterStatus { get; set; } = null!;
        public string CenterType { get; set; } = null!;
        public TimeOnly? OpenTime { get; set; }
        public TimeOnly? CloseTime { get; set; }
        public decimal Rating { get; set; }
        public string MainImage { get; set; } = null!;

    }
}
