using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Data;

[Index("Token", Name = "UQ__CenterIn__1EB4F817F3B2E391", IsUnique = true)]
public partial class CenterInvitation
{
    [Key]
    public int InvitationId { get; set; }

    public int CenterId { get; set; }

    [StringLength(150)]
    public string Token { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime ExpiresAt { get; set; }

    public DateTime? AcceptedAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    public int? UserAcceptedId { get; set; }

    public bool IsUsed { get; set; }

    [ForeignKey("CenterId")]
    [InverseProperty("CenterInvitations")]
    public virtual Center Center { get; set; } = null!;

    [ForeignKey("UserAcceptedId")]
    [InverseProperty("CenterInvitations")]
    public virtual User? UserAccepted { get; set; }
}
