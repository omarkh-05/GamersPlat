using System;
using System.Collections.Generic;

namespace Data;

public partial class CenterInvitation
{
    public int InvitationId { get; set; }

    public int CenterId { get; set; }

    public string Token { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }

    public DateTime? AcceptedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public int? UserAcceptedId { get; set; }

    public bool IsUsed { get; set; }

    public virtual Center Center { get; set; } = null!;

    public virtual User? UserAccepted { get; set; }
}
