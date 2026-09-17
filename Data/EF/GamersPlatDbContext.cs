using System;
using System.Collections.Generic;
using Data;
using Microsoft.EntityFrameworkCore;

namespace Data.EF;

public partial class GamersPlatDbContext : DbContext
{
    public GamersPlatDbContext()
    {
    }

    public GamersPlatDbContext(DbContextOptions<GamersPlatDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Center> Centers { get; set; }

    public virtual DbSet<CenterImage> CenterImages { get; set; }

    public virtual DbSet<CenterInvitation> CenterInvitations { get; set; }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<Device> Devices { get; set; }

    public virtual DbSet<EmailVerificationToken> EmailVerificationTokens { get; set; }

    public virtual DbSet<Game> Games { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Offer> Offers { get; set; }

    public virtual DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

    public virtual DbSet<PointTransaction> PointTransactions { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<ResourcesType> ResourcesTypes { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<Session> Sessions { get; set; }

    public virtual DbSet<SessionParticipant> SessionParticipants { get; set; }

    public virtual DbSet<Tournament> Tournaments { get; set; }

    public virtual DbSet<TournamentPlayer> TournamentPlayers { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.;Database=GamersPlatDB;User Id=sa;Password=123456;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.AuditId).HasName("PK__AuditLog__A17F23983077B4EE");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.User).WithMany(p => p.AuditLogs).HasConstraintName("FK__AuditLogs__UserI__395884C4");
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__Bookings__73951AED48B1CB1C");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())", "DF__Bookings__Create__05D8E0BE");
            entity.Property(e => e.EarnedPoints).HasDefaultValue(0, "DF__Bookings__Earned__03F0984C");
            entity.Property(e => e.Status).HasDefaultValue("Pending", "DF__Bookings__Bookin__04E4BC85");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(NULL)", "DF_Bookings_UpdatedAt");

            entity.HasOne(d => d.Center).WithMany(p => p.Bookings)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Bookings__Center__07C12930");

            entity.HasOne(d => d.Offer).WithMany(p => p.Bookings).HasConstraintName("FK_Bookings_Offers");

            entity.HasOne(d => d.ResourcesType).WithMany(p => p.Bookings)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Bookings__Device__08B54D69");

            entity.HasOne(d => d.User).WithMany(p => p.Bookings).HasConstraintName("FK__Bookings__UserId__06CD04F7");
        });

        modelBuilder.Entity<Center>(entity =>
        {
            entity.HasKey(e => e.CenterId).HasName("PK__Centers__398FC7F7D91C2404");

            entity.Property(e => e.CenterStatus).HasDefaultValue("Pending", "DF__Centers__CenterS__656C112C");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())", "DF__Centers__Created__6754599E");
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF__Centers__IsActiv__66603565");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(NULL)", "DF_Centers_UpdatedAt");

            entity.HasOne(d => d.City).WithMany(p => p.Centers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Centers__CityId__6A30C649");

            entity.HasOne(d => d.OwnerUser).WithMany(p => p.Centers).HasConstraintName("FK__Centers__OwnerUs__68487DD7");
        });

        modelBuilder.Entity<CenterImage>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("PK__CenterIm__7516F70C23F048F9");

            entity.HasIndex(e => e.CenterId, "UX_CenterImages_OneMain")
                .IsUnique()
                .HasFilter("([IsMain]=(1))");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())", "DF_CenterImages_CreatedAt");

            entity.HasOne(d => d.Center).WithOne(p => p.CenterImage)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CenterIma__Cente__6E01572D");
        });

        modelBuilder.Entity<CenterInvitation>(entity =>
        {
            entity.HasKey(e => e.InvitationId).HasName("PK__CenterIn__033C8DCF3A9D5841");

            entity.HasOne(d => d.Center).WithMany(p => p.CenterInvitations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CenterInv__Cente__778AC167");

            entity.HasOne(d => d.UserAccepted).WithMany(p => p.CenterInvitations).HasConstraintName("FK_CenterInvitations_Users");
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.CityId).HasName("PK__Cities__F2D21B763DE17238");

            entity.HasOne(d => d.Country).WithMany(p => p.Cities)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Cities__CountryI__4D94879B");
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.CountryId).HasName("PK__Countrie__10D1609F5B834872");
        });

        modelBuilder.Entity<Device>(entity =>
        {
            entity.Property(e => e.DeviceId).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<EmailVerificationToken>(entity =>
        {
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())", "DF_EmailVerificationTokens_CreatedAt");
            entity.Property(e => e.UsedAt).HasDefaultValueSql("(NULL)", "DF_EmailVerificationTokens_UsedAt");

            entity.HasOne(d => d.User).WithMany(p => p.EmailVerificationTokens)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmailVerificationTokens_Users");
        });

        modelBuilder.Entity<Game>(entity =>
        {
            entity.Property(e => e.GameName).IsFixedLength();
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E127B77B9B3");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())", "DF__Notificat__Creat__245D67DE");

            entity.HasOne(d => d.Center).WithMany(p => p.Notifications).HasConstraintName("FK__Notificat__Cente__2645B050");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Notificat__UserI__25518C17");
        });

        modelBuilder.Entity<Offer>(entity =>
        {
            entity.HasKey(e => e.OfferId).HasName("PK__Offers__8EBCF091DB065D09");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())", "DF__Offers__CreatedA__208CD6FA");
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF__Offers__IsActive__1F98B2C1");

            entity.HasOne(d => d.Center).WithMany(p => p.Offers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Offers__CenterId__2180FB33");
        });

        modelBuilder.Entity<PasswordResetToken>(entity =>
        {
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())", "DF_PasswordResetTokens_CreatedAt");
            entity.Property(e => e.UserAt).HasDefaultValueSql("(NULL)", "DF_PasswordResetTokens_UserAt");

            entity.HasOne(d => d.User).WithMany(p => p.PasswordResetTokens)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PasswordResetTokens_Users");
        });

        modelBuilder.Entity<PointTransaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("PK__PointTra__55433A6BE76FB2AF");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())", "DF__PointTran__Creat__339FAB6E");

            entity.HasOne(d => d.Center).WithMany(p => p.PointTransactions).HasConstraintName("FK__PointTran__Cente__3587F3E0");

            entity.HasOne(d => d.User).WithMany(p => p.PointTransactions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PointTran__UserI__3493CFA7");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.TokenId).HasName("PK__RefreshT__3214EC07049C36DC");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())", "DF__RefreshTo__Creat__60A75C0F");

            entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RefreshTo__UserI__628FA481");
        });

        modelBuilder.Entity<ResourcesType>(entity =>
        {
            entity.HasKey(e => e.ResourcesTypeId).HasName("PK__DeviceTy__07A6C7F6EB9269A4");

            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF__DeviceTyp__IsAct__7B5B524B");
            entity.Property(e => e.RoomType).IsFixedLength();

            entity.HasOne(d => d.Center).WithMany(p => p.ResourcesTypes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DeviceTyp__Cente__7C4F7684");

            entity.HasOne(d => d.Device).WithMany(p => p.ResourcesTypes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DeviceTypes_Devices");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PK__Reviews__74BC79CE827CEE8F");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())", "DF__Reviews__Created__19DFD96B");

            entity.HasOne(d => d.Booking).WithMany(p => p.Reviews)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reviews_Bookings");

            entity.HasOne(d => d.Center).WithMany(p => p.Reviews)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reviews__CenterI__1BC821DD");

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reviews__UserId__1AD3FDA4");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE1AA810D58A");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.Property(e => e.ServiceId).ValueGeneratedOnAdd();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())", "DF_Services_CreatedAt");
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_Services_IsActive");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(NULL)", "DF_Services_UpdatedAt");

            entity.HasOne(d => d.Center).WithMany(p => p.Services)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Services_Centers");
        });

        modelBuilder.Entity<Session>(entity =>
        {
            entity.HasKey(e => e.SessionId).HasName("PK__Sessions__C9F492902AF2EE27");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())", "DF__Sessions__Create__0E6E26BF");
            entity.Property(e => e.SessionStatus).HasDefaultValue("Open", "DF__Sessions__Sessio__0D7A0286");

            entity.HasOne(d => d.Center).WithMany(p => p.Sessions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Sessions__Center__0F624AF8");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.Sessions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Sessions__Create__10566F31");

            entity.HasOne(d => d.Device).WithMany(p => p.Sessions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Sessions_Devices");

            entity.HasOne(d => d.Game).WithMany(p => p.Sessions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Sessions_Games");
        });

        modelBuilder.Entity<SessionParticipant>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SessionP__3214EC0789CA5C8A");

            entity.Property(e => e.JoinedAt).HasDefaultValueSql("(getdate())", "DF__SessionPa__Joine__14270015");

            entity.HasOne(d => d.Session).WithOne(p => p.SessionParticipant)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SessionPa__Sessi__151B244E");

            entity.HasOne(d => d.User).WithMany(p => p.SessionParticipants)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SessionPa__UserI__160F4887");
        });

        modelBuilder.Entity<Tournament>(entity =>
        {
            entity.HasKey(e => e.TournamentId).HasName("PK__Tourname__AC631313EFC25780");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())", "DF__Tournamen__Creat__29221CFB");

            entity.HasOne(d => d.Center).WithMany(p => p.Tournaments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Tournamen__Cente__2A164134");

            entity.HasOne(d => d.Device).WithMany(p => p.Tournaments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tournaments_Devices");

            entity.HasOne(d => d.Game).WithMany(p => p.Tournaments).HasConstraintName("FK_Tournaments_Games");

            entity.HasOne(d => d.WinnerUser).WithMany(p => p.Tournaments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Tournamen__Winne__2B0A656D");
        });

        modelBuilder.Entity<TournamentPlayer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tourname__3214EC07CD4D5238");

            entity.Property(e => e.JoinedAt).HasDefaultValueSql("(getdate())", "DF__Tournamen__Joine__2EDAF651");

            entity.HasOne(d => d.Tournament).WithMany(p => p.TournamentPlayers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Tournamen__Tourn__2FCF1A8A");

            entity.HasOne(d => d.User).WithMany(p => p.TournamentPlayers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Tournamen__UserI__30C33EC3");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4CF896EB48");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())", "DF__Users__CreatedAt__5441852A");
            entity.Property(e => e.EmailVerified).HasDefaultValue(false, "DF_Users_EmailVerified");
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF__Users__IsActive__534D60F1");
            entity.Property(e => e.LastLoginAt).HasDefaultValueSql("(NULL)", "DF_Users_LastLoginAt");
            entity.Property(e => e.PhoneVerified).HasDefaultValue(false, "DF_Users_PhoneVerified");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(NULL)", "DF_Users_UpdatedAt");

            entity.HasOne(d => d.City).WithMany(p => p.Users)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Cities");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserRole__3214EC075BD6836E");

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserRoles__RoleI__5CD6CB2B");

            entity.HasOne(d => d.User).WithMany(p => p.UserRoles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserRoles__UserI__5BE2A6F2");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
