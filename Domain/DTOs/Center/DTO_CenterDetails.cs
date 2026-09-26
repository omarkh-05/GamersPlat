using Domain.DTOs.Offer;
using Domain.DTOs.Resource;
using Domain.DTOs.Session;
using Domain.DTOs.Tournuments;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DTOs.Center
{
    public class DTO_CenterDetails
    {
        public int CenterId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public string CenterType { get; set; } = string.Empty;
        public TimeOnly OpenTime { get; set; }
        public TimeOnly CloseTime { get; set; }

        public double Rating { get; set; }
        public int ReviewCount { get; set; }

        public List<string>? Images { get; set; } = new();
        public List<string> Services { get; set; } = new();

        public List<DTO_TournamentsInCenterDetails> Tournaments { get; set; } = new();
        public List<DTO_SessionInCenterDetails> Sessions { get; set; } = new();
        public List<DTO_OfferInCenterDetails> Offers { get; set; } = new();

        public List<DTO_ResourceType> VipRooms { get; set; } = new();
        public List<DTO_ResourceType> NormalRooms { get; set; } = new();
    }
}
