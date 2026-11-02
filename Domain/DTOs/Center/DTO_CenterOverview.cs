using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DTOs.Center
{
    public class DTO_CenterOverview
    {
        public string CenterName { get; set; } = null!;
        public string CityName { get; set; } = null!;
        public string? CenterStatus { get; set; }
        public string? CenterAddress { get; set; }
        public string? ImageUrl { get; set; }

        public int ResourceCount { get; set; }
        public int Units { get; set; }

        public int BookingsCount { get; set; }
        public decimal Revenue { get; set; }
    }
}
