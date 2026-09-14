using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DTOs.Player
{
    public class DTO_PlayerTournamentInfo
    {
        public string CenterName { get; set; } = "";
        public string TournamentName { get; set; } = "";
        public string GameName { get; set; } = "";
        public string DeviceName { get; set; } = "";
        public string? WinnerUserId { get; set; } = "";
        public int RewardPoints { get; set; } = 0;
        public DateTime JoinedAt { get; set; }
    }
}
