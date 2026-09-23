using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DTOs.Tournuments
{
    public class DTO_TournamentsInCenterDetails
    {
        public int TournamentId { get; set; }
        public string TournamentName { get; set; } = string.Empty;
        public string GameName { get; set; } = string.Empty;
        public decimal PrizePool { get; set; }
        public int SlotsTaken { get; set; }
        public int? MaxPlayers { get; set; }
        public decimal EntryFee { get; set; }
        public DateTime StartDate { get; set; }
    }
}
