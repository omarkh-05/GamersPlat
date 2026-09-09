using System;
using System.Collections.Generic;

namespace Data;

public partial class Tournament
{
    public int TournamentId { get; set; }

    public int CenterId { get; set; }

    public string TournamentName { get; set; } = null!;

    public short? GameId { get; set; }

    public byte DeviceId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public int? MaxPlayers { get; set; }

    public int WinnerUserId { get; set; }

    public int RewardPoints { get; set; }

    public string Description { get; set; } = null!;

    public DateTime? UpdatedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Center Center { get; set; } = null!;

    public virtual Device Device { get; set; } = null!;

    public virtual Game? Game { get; set; }

    public virtual ICollection<TournamentPlayer> TournamentPlayers { get; set; } = new List<TournamentPlayer>();

    public virtual User WinnerUser { get; set; } = null!;
}
