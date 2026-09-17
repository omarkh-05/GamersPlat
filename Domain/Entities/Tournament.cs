using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Data;

public partial class Tournament
{
    [Key]
    public int TournamentId { get; set; }

    public int CenterId { get; set; }

    [StringLength(150)]
    public string TournamentName { get; set; } = null!;

    public short? GameId { get; set; }

    public byte DeviceId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime StartDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime EndDate { get; set; }

    public int? MaxPlayers { get; set; }

    public int WinnerUserId { get; set; }

    public int RewardPoints { get; set; }

    public string Description { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("CenterId")]
    [InverseProperty("Tournaments")]
    public virtual Center Center { get; set; } = null!;

    [ForeignKey("DeviceId")]
    [InverseProperty("Tournaments")]
    public virtual Device Device { get; set; } = null!;

    [ForeignKey("GameId")]
    [InverseProperty("Tournaments")]
    public virtual Game? Game { get; set; }

    [InverseProperty("Tournament")]
    public virtual ICollection<TournamentPlayer> TournamentPlayers { get; set; } = new List<TournamentPlayer>();

    [ForeignKey("WinnerUserId")]
    [InverseProperty("Tournaments")]
    public virtual User WinnerUser { get; set; } = null!;
}
