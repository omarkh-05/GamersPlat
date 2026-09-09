using System;
using System.Collections.Generic;

namespace Data;

public partial class User
{
    public int UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? Email { get; set; }

    public int Points { get; set; }

    public bool IsActive { get; set; }

    public bool? PhoneVerified { get; set; }

    public bool? EmailVerified { get; set; }

    public short CityId { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? LastLoginAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual ICollection<CenterInvitation> CenterInvitations { get; set; } = new List<CenterInvitation>();

    public virtual ICollection<Center> Centers { get; set; } = new List<Center>();

    public virtual City City { get; set; } = null!;

    public virtual ICollection<EmailVerificationToken> EmailVerificationTokens { get; set; } = new List<EmailVerificationToken>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<PasswordResetToken> PasswordResetTokens { get; set; } = new List<PasswordResetToken>();

    public virtual ICollection<PointTransaction> PointTransactions { get; set; } = new List<PointTransaction>();

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<SessionParticipant> SessionParticipants { get; set; } = new List<SessionParticipant>();

    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();

    public virtual ICollection<TournamentPlayer> TournamentPlayers { get; set; } = new List<TournamentPlayer>();

    public virtual ICollection<Tournament> Tournaments { get; set; } = new List<Tournament>();

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
