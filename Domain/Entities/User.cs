using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Data;

[Index("Email", Name = "UQ_Users_Email", IsUnique = true)]
[Index("PhoneNumber", Name = "UQ_Users_PhoneNumber", IsUnique = true)]
[Index("Email", Name = "UQ__Users__A9D105343662952E", IsUnique = true)]
public partial class User
{
    [Key]
    public int UserId { get; set; }

    [StringLength(150)]
    public string FullName { get; set; } = null!;

    [StringLength(30)]
    public string PhoneNumber { get; set; } = null!;

    [StringLength(255)]
    public string PasswordHash { get; set; } = null!;

    [StringLength(255)]
    public string? Email { get; set; }

    public int Points { get; set; }

    public bool IsActive { get; set; }

    public bool? PhoneVerified { get; set; }

    public bool? EmailVerified { get; set; }

    public short CityId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastLoginAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    [InverseProperty("User")]
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    [InverseProperty("UserAccepted")]
    public virtual ICollection<CenterInvitation> CenterInvitations { get; set; } = new List<CenterInvitation>();

    [InverseProperty("OwnerUser")]
    public virtual ICollection<Center> Centers { get; set; } = new List<Center>();

    [ForeignKey("CityId")]
    [InverseProperty("Users")]
    public virtual City City { get; set; } = null!;

    [InverseProperty("User")]
    public virtual ICollection<EmailVerificationToken> EmailVerificationTokens { get; set; } = new List<EmailVerificationToken>();

    [InverseProperty("User")]
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    [InverseProperty("User")]
    public virtual ICollection<PasswordResetToken> PasswordResetTokens { get; set; } = new List<PasswordResetToken>();

    [InverseProperty("User")]
    public virtual ICollection<PointTransaction> PointTransactions { get; set; } = new List<PointTransaction>();

    [InverseProperty("User")]
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    [InverseProperty("User")]
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    [InverseProperty("User")]
    public virtual ICollection<SessionParticipant> SessionParticipants { get; set; } = new List<SessionParticipant>();

    [InverseProperty("CreatedByUser")]
    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();

    [InverseProperty("User")]
    public virtual Staff? Staff { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<TournamentPlayer> TournamentPlayers { get; set; } = new List<TournamentPlayer>();

    [InverseProperty("WinnerUser")]
    public virtual ICollection<Tournament> Tournaments { get; set; } = new List<Tournament>();

    [InverseProperty("User")]
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
