using System;
using System.Collections.Generic;

namespace Data;

public partial class TournamentPlayer
{
    public int Id { get; set; }

    public int TournamentId { get; set; }

    public int UserId { get; set; }

    public DateTime JoinedAt { get; set; }

    public virtual Tournament Tournament { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
