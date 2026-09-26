using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Data;

public partial class EmailVerificationToken
{
    [Key]
    public int TokenIdId { get; set; }

    public int UserId { get; set; }

    [StringLength(150)]
    public string TokenHash { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }

    public DateTime? UsedAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("EmailVerificationTokens")]
    public virtual User User { get; set; } = null!;
}
