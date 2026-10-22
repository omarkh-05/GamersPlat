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
        public string Address { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public TimeOnly? OpenTime { get; set; }
        public TimeOnly? CloseTime { get; set; }


        public string? MainImage { get; set; } = null!;
        public List<string>? GalleryImages { get; set; } = new();
        public List<string> Services { get; set; } = new();


        public List<DTO_ResourceType> Vip { get; set; } = new();
        public List<DTO_ResourceType> Normal { get; set; } = new();

        /*
        public double Rating { get; set; }
        public int ReviewCount { get; set; }
        public List<DTO_TournamentsInCenterDetails> Tournaments { get; set; } = new();
        public List<DTO_SessionInCenterDetails> Sessions { get; set; } = new();
        public List<DTO_OfferInCenterDetails> Offers { get; set; } = new();
        */
    }
}
