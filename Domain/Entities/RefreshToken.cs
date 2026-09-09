using System;
using System.Collections.Generic;

namespace Data;

public partial class RefreshToken
{
    public int TokenId { get; set; }

    public int UserId { get; set; }

    public string TokenHash { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    public string? RevokedByIp { get; set; }

    public string? UserAgent { get; set; }

    public bool IsRevoked { get; set; }

    public virtual User User { get; set; } = null!;
}
