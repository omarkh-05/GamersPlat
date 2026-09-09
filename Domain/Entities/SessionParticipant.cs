using System;
using System.Collections.Generic;

namespace Data;

public partial class SessionParticipant
{
    public int Id { get; set; }

    public int SessionId { get; set; }

    public int UserId { get; set; }

    public DateTime JoinedAt { get; set; }

    public virtual Session Session { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
