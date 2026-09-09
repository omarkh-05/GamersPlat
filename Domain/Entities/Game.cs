using System;
using System.Collections.Generic;

namespace Data;

public partial class Game
{
    public short GameId { get; set; }

    public string GameName { get; set; } = null!;

    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();

    public virtual ICollection<Tournament> Tournaments { get; set; } = new List<Tournament>();
}
