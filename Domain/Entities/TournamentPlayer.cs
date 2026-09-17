using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Data;

[Index("TournamentId", "UserId", Name = "UQ_TournamentPlayers_TournamentId_UserId", IsUnique = true)]
[Index("TournamentId", "UserId", Name = "UQ__Tourname__7D1B9FD62AB46169", IsUnique = true)]
public partial class TournamentPlayer
{
    [Key]
    public int Id { get; set; }

    public int TournamentId { get; set; }

    public int UserId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime JoinedAt { get; set; }

    [ForeignKey("TournamentId")]
    [InverseProperty("TournamentPlayers")]
    public virtual Tournament Tournament { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("TournamentPlayers")]
    public virtual User User { get; set; } = null!;
}
