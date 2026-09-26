using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Data;

public partial class PasswordResetToken
{
    [Key]
    public int TokenId { get; set; }

    public int UserId { get; set; }

    [StringLength(150)]
    public string TokenHash { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }

    public DateTime? UserAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("PasswordResetTokens")]
    public virtual User User { get; set; } = null!;
}
