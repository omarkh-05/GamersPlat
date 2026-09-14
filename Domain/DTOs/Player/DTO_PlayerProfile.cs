using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DTOs.Player
{
    public class DTO_PlayerProfile
    {
        // User Information
        public string? PhoneNumber { get; set; }

        public string? FullName { get; set; }

        public string? Email { get; set; }

        public int CityId { get; set; }

        public int? Points { get; set; } = 0;

        public bool IsActive { get; set; } = false;

        public bool PhoneVerified { get; set; } = false;

        public bool EmailVerified { get; set; } = false;

        // Player Statistics
        public int TotalBookings { get; set; } = 0;

        public int TotalTournaments { get; set; } = 0;

        public decimal TotalPaid { get; set; } = 0;

        public int? MostPlayedCenterId { get; set; } = 0;

        public string? MostPlayedCenterName { get; set; } = "";
    }
}
