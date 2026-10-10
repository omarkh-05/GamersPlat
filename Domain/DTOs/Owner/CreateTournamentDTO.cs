using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.DTOs.Owner
{
    public class CreateTournamentDTO
    {
        [Range(1, int.MaxValue)]
        public int CenterId { get; set; }


    [Required]
        [StringLength(150)]
        public string TournamentName { get; set; } = string.Empty;

        public short? GameId { get; set; }

        [Range(1, byte.MaxValue)]
        public byte DeviceId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public int? MaxPlayers { get; set; }

        [Range(1, int.MaxValue)]
        public int WinnerUserId { get; set; }

        [Range(0, int.MaxValue)]
        public int RewardPoints { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;
    }


}
