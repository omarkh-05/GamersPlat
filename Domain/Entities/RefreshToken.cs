using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Data;

[Index("TokenHash", Name = "UQ__RefreshT__1EB4F81703D8DE17", IsUnique = true)]
public partial class RefreshToken
{
    [Key]
    public int TokenId { get; set; }

    public int UserId { get; set; }

    [StringLength(500)]
    public string TokenHash { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime ExpiresAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    [StringLength(100)]
    public string? RevokedByIp { get; set; }

    [StringLength(50)]
    public string? UserAgent { get; set; }

    public bool IsRevoked { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("RefreshTokens")]
    public virtual User User { get; set; } = null!;
}
