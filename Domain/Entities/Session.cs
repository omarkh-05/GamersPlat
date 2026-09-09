using System;
using System.Collections.Generic;

namespace Data;

public partial class Session
{
    public int SessionId { get; set; }

    public int CenterId { get; set; }

    public short GameId { get; set; }

    public byte DeviceId { get; set; }

    public DateOnly SessionDate { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public short MaxPlayers { get; set; }

    public int CreatedByUserId { get; set; }

    public string SessionStatus { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual Center Center { get; set; } = null!;

    public virtual User CreatedByUser { get; set; } = null!;

    public virtual Device Device { get; set; } = null!;

    public virtual Game Game { get; set; } = null!;

    public virtual SessionParticipant? SessionParticipant { get; set; }
}
