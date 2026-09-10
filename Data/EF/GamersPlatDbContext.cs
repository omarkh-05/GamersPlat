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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) => optionsBuilder.UseSqlServer(Connection.ConnectionString);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.AuditId).HasName("PK__AuditLog__A17F23983077B4EE");

            entity.Property(e => e.ActionDone).HasMaxLength(100);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EntityName).HasMaxLength(100);
            entity.Property(e => e.Ipaddress)
                .HasMaxLength(45)
                .HasColumnName("IPAddress");
            entity.Property(e => e.UserAgent).HasMaxLength(300);

            entity.HasOne(d => d.User).WithMany(p => p.AuditLogs)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__AuditLogs__UserI__395884C4");
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.BookingId).HasName("PK__Bookings__73951AED48B1CB1C");

            entity.Property(e => e.CancellationReason).HasMaxLength(500);
            entity.Property(e => e.CancelledAt).HasColumnType("datetime");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())", "DF__Bookings__Create__05D8E0BE")
                .HasColumnType("datetime");
            entity.Property(e => e.CustomerName).HasMaxLength(150);
            entity.Property(e => e.DiscountType).HasMaxLength(50);
            entity.Property(e => e.EarnedPoints).HasDefaultValue(0, "DF__Bookings__Earned__03F0984C");
            entity.Property(e => e.GameName).HasMaxLength(300);
            entity.Property(e => e.PhoneNumber).HasMaxLength(30);
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasDefaultValue("Pending", "DF__Bookings__Bookin__04E4BC85");
            entity.Property(e => e.Subtotal).HasColumnType("decimal(7, 2)");
            entity.Property(e => e.TotalPrice).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(NULL)", "DF_Bookings_UpdatedAt")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Center).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.CenterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Bookings__Center__07C12930");

            entity.HasOne(d => d.Offer).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.OfferId)
                .HasConstraintName("FK_Bookings_Offers");

            entity.HasOne(d => d.ResourcesType).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.ResourcesTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Bookings__Device__08B54D69");

            entity.HasOne(d => d.User).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Bookings__UserId__06CD04F7");
        });

        modelBuilder.Entity<Center>(entity =>
        {
            entity.HasKey(e => e.CenterId).HasName("PK__Centers__398FC7F7D91C2404");

            entity.Property(e => e.CenterAddress).HasMaxLength(255);
            entity.Property(e => e.CenterName).HasMaxLength(150);
            entity.Property(e => e.CenterStatus)
                .HasMaxLength(50)
                .HasDefaultValue("Pending", "DF__Centers__CenterS__656C112C");
            entity.Property(e => e.CenterType).HasMaxLength(50);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())", "DF__Centers__Created__6754599E")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF__Centers__IsActiv__66603565");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(NULL)", "DF_Centers_UpdatedAt")
                .HasColumnType("datetime");

            entity.HasOne(d => d.City).WithMany(p => p.Centers)
                .HasForeignKey(d => d.CityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Centers__CityId__6A30C649");

            entity.HasOne(d => d.OwnerUser).WithMany(p => p.Centers)
                .HasForeignKey(d => d.OwnerUserId)
                .HasConstraintName("FK__Centers__OwnerUs__68487DD7");
        });

        modelBuilder.Entity<CenterImage>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("PK__CenterIm__7516F70C23F048F9");

            entity.HasIndex(e => e.CenterId, "UX_CenterImages_OneMain")
                .IsUnique()
                .HasFilter("([IsMain]=(1))");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())", "DF_CenterImages_CreatedAt")
                .HasColumnType("datetime");
            entity.Property(e => e.ImageUrl).HasMaxLength(500);

            entity.HasOne(d => d.Center).WithOne(p => p.CenterImage)
                .HasForeignKey<CenterImage>(d => d.CenterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CenterIma__Cente__6E01572D");
        });

        modelBuilder.Entity<CenterInvitation>(entity =>
        {
            entity.HasKey(e => e.InvitationId).HasName("PK__CenterIn__033C8DCF3A9D5841");

            entity.HasIndex(e => e.Token, "UQ__CenterIn__1EB4F817F3B2E391").IsUnique();

            entity.Property(e => e.CreatedAt).HasColumnType("datetime");
            entity.Property(e => e.ExpiresAt).HasColumnType("datetime");
            entity.Property(e => e.Token).HasMaxLength(150);

            entity.HasOne(d => d.Center).WithMany(p => p.CenterInvitations)
                .HasForeignKey(d => d.CenterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CenterInv__Cente__778AC167");

            entity.HasOne(d => d.UserAccepted).WithMany(p => p.CenterInvitations)
                .HasForeignKey(d => d.UserAcceptedId)
                .HasConstraintName("FK_CenterInvitations_Users");
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.CityId).HasName("PK__Cities__F2D21B763DE17238");

            entity.HasIndex(e => new { e.CountryId, e.Name }, "UQ_Cities_CountryId_Name").IsUnique();

            entity.HasIndex(e => new { e.Name, e.CountryId }, "UQ__Cities__696C4FEC80312ADA").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(100);

            entity.HasOne(d => d.Country).WithMany(p => p.Cities)
                .HasForeignKey(d => d.CountryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Cities__CountryI__4D94879B");
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.CountryId).HasName("PK__Countrie__10D1609F5B834872");

            entity.HasIndex(e => e.Name, "UQ_Countries_Name").IsUnique();

            entity.HasIndex(e => e.Name, "UQ__Countrie__E056F2015A203A28").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Device>(entity =>
        {
            entity.Property(e => e.DeviceId).ValueGeneratedOnAdd();
            entity.Property(e => e.DeviceName).HasMaxLength(50);
        });

        modelBuilder.Entity<EmailVerificationToken>(entity =>
        {
            entity.HasKey(e => e.TokenIdId);

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())", "DF_EmailVerificationTokens_CreatedAt")
                .HasColumnType("datetime");
            entity.Property(e => e.TokenHash).HasMaxLength(150);
            entity.Property(e => e.UsedAt).HasDefaultValueSql("(NULL)", "DF_EmailVerificationTokens_UsedAt");

            entity.HasOne(d => d.User).WithMany(p => p.EmailVerificationTokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EmailVerificationTokens_Users");
        });

        modelBuilder.Entity<Game>(entity =>
        {
            entity.Property(e => e.GameName)
                .HasMaxLength(400)
                .IsFixedLength();
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E127B77B9B3");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())", "DF__Notificat__Creat__245D67DE")
                .HasColumnType("datetime");
            entity.Property(e => e.ReadAt).HasColumnType("datetime");
            entity.Property(e => e.ReferenceType).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(150);
            entity.Property(e => e.Type).HasMaxLength(50);

            entity.HasOne(d => d.Center).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.CenterId)
                .HasConstraintName("FK__Notificat__Cente__2645B050");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Notificat__UserI__25518C17");
        });

        modelBuilder.Entity<Offer>(entity =>
        {
            entity.HasKey(e => e.OfferId).HasName("PK__Offers__8EBCF091DB065D09");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())", "DF__Offers__CreatedA__208CD6FA")
                .HasColumnType("datetime");
            entity.Property(e => e.DiscountType).HasMaxLength(50);
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF__Offers__IsActive__1F98B2C1");
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(150);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Center).WithMany(p => p.Offers)
                .HasForeignKey(d => d.CenterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Offers__CenterId__2180FB33");
        });

        modelBuilder.Entity<PasswordResetToken>(entity =>
        {
            entity.HasKey(e => e.TokenId);

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())", "DF_PasswordResetTokens_CreatedAt")
                .HasColumnType("datetime");
            entity.Property(e => e.TokenHash).HasMaxLength(150);
            entity.Property(e => e.UserAt).HasDefaultValueSql("(NULL)", "DF_PasswordResetTokens_UserAt");

            entity.HasOne(d => d.User).WithMany(p => p.PasswordResetTokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PasswordResetTokens_Users");
        });

        modelBuilder.Entity<PointTransaction>(entity =>
        {
            entity.HasKey(e => e.TransactionId).HasName("PK__PointTra__55433A6BE76FB2AF");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())", "DF__PointTran__Creat__339FAB6E")
                .HasColumnType("datetime");
            entity.Property(e => e.PointType).HasMaxLength(50);
            entity.Property(e => e.ReferenceType).HasMaxLength(50);

            entity.HasOne(d => d.Center).WithMany(p => p.PointTransactions)
                .HasForeignKey(d => d.CenterId)
                .HasConstraintName("FK__PointTran__Cente__3587F3E0");

            entity.HasOne(d => d.User).WithMany(p => p.PointTransactions)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__PointTran__UserI__3493CFA7");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.TokenId).HasName("PK__RefreshT__3214EC07049C36DC");

            entity.HasIndex(e => e.TokenHash, "UQ__RefreshT__1EB4F81703D8DE17").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())", "DF__RefreshTo__Creat__60A75C0F")
                .HasColumnType("datetime");
            entity.Property(e => e.ExpiresAt).HasColumnType("datetime");
            entity.Property(e => e.RevokedByIp).HasMaxLength(100);
            entity.Property(e => e.TokenHash).HasMaxLength(500);
            entity.Property(e => e.UserAgent).HasMaxLength(50);

            entity.HasOne(d => d.User).WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RefreshTo__UserI__628FA481");
        });

        modelBuilder.Entity<ResourcesType>(entity =>
        {
            entity.HasKey(e => e.ResourcesTypeId).HasName("PK__DeviceTy__07A6C7F6EB9269A4");

            entity.HasIndex(e => new { e.CenterId, e.DeviceId }, "UQ__DeviceTy__F8D8EF08BA9EF532").IsUnique();

            entity.Property(e => e.HourlyPrice).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF__DeviceTyp__IsAct__7B5B524B");
            entity.Property(e => e.RoomType)
                .HasMaxLength(10)
                .IsFixedLength();

            entity.HasOne(d => d.Center).WithMany(p => p.ResourcesTypes)
                .HasForeignKey(d => d.CenterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__DeviceTyp__Cente__7C4F7684");

            entity.HasOne(d => d.Device).WithMany(p => p.ResourcesTypes)
                .HasForeignKey(d => d.DeviceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DeviceTypes_Devices");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PK__Reviews__74BC79CE827CEE8F");

            entity.HasIndex(e => new { e.UserId, e.BookingId }, "UQ_Reviews_UserId_BookingId").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())", "DF__Reviews__Created__19DFD96B")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Booking).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.BookingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reviews_Bookings");

            entity.HasOne(d => d.Center).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.CenterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reviews__CenterI__1BC821DD");

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reviews__UserId__1AD3FDA4");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE1AA810D58A");

            entity.HasIndex(e => e.RoleName, "UQ_Roles_RoleName").IsUnique();

            entity.HasIndex(e => e.RoleName, "UQ__Roles__8A2B6160890B126E").IsUnique();

            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.Property(e => e.ServiceId).ValueGeneratedOnAdd();
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())", "DF_Services_CreatedAt")
                .HasColumnType("datetime");
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF_Services_IsActive");
            entity.Property(e => e.Name).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(NULL)", "DF_Services_UpdatedAt")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Center).WithMany(p => p.Services)
                .HasForeignKey(d => d.CenterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Services_Centers");
        });

        modelBuilder.Entity<Session>(entity =>
        {
            entity.HasKey(e => e.SessionId).HasName("PK__Sessions__C9F492902AF2EE27");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())", "DF__Sessions__Create__0E6E26BF")
                .HasColumnType("datetime");
            entity.Property(e => e.SessionStatus)
                .HasMaxLength(50)
                .HasDefaultValue("Open", "DF__Sessions__Sessio__0D7A0286");

            entity.HasOne(d => d.Center).WithMany(p => p.Sessions)
                .HasForeignKey(d => d.CenterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Sessions__Center__0F624AF8");

            entity.HasOne(d => d.CreatedByUser).WithMany(p => p.Sessions)
                .HasForeignKey(d => d.CreatedByUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Sessions__Create__10566F31");

            entity.HasOne(d => d.Device).WithMany(p => p.Sessions)
                .HasForeignKey(d => d.DeviceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Sessions_Devices");

            entity.HasOne(d => d.Game).WithMany(p => p.Sessions)
                .HasForeignKey(d => d.GameId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Sessions_Games");
        });

        modelBuilder.Entity<SessionParticipant>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SessionP__3214EC0789CA5C8A");

            entity.HasIndex(e => new { e.SessionId, e.UserId }, "UQ_SessionParticipants_SessionId_UserId").IsUnique();

            entity.HasIndex(e => e.SessionId, "UQ__SessionP__C9F49291B016FBF8").IsUnique();

            entity.Property(e => e.JoinedAt)
                .HasDefaultValueSql("(getdate())", "DF__SessionPa__Joine__14270015")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Session).WithOne(p => p.SessionParticipant)
                .HasForeignKey<SessionParticipant>(d => d.SessionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SessionPa__Sessi__151B244E");

            entity.HasOne(d => d.User).WithMany(p => p.SessionParticipants)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SessionPa__UserI__160F4887");
        });

        modelBuilder.Entity<Tournament>(entity =>
        {
            entity.HasKey(e => e.TournamentId).HasName("PK__Tourname__AC631313EFC25780");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())", "DF__Tournamen__Creat__29221CFB")
                .HasColumnType("datetime");
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.TournamentName).HasMaxLength(150);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

            entity.HasOne(d => d.Center).WithMany(p => p.Tournaments)
                .HasForeignKey(d => d.CenterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Tournamen__Cente__2A164134");

            entity.HasOne(d => d.Device).WithMany(p => p.Tournaments)
                .HasForeignKey(d => d.DeviceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tournaments_Devices");

            entity.HasOne(d => d.Game).WithMany(p => p.Tournaments)
                .HasForeignKey(d => d.GameId)
                .HasConstraintName("FK_Tournaments_Games");

            entity.HasOne(d => d.WinnerUser).WithMany(p => p.Tournaments)
                .HasForeignKey(d => d.WinnerUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Tournamen__Winne__2B0A656D");
        });

        modelBuilder.Entity<TournamentPlayer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tourname__3214EC07CD4D5238");

            entity.HasIndex(e => new { e.TournamentId, e.UserId }, "UQ_TournamentPlayers_TournamentId_UserId").IsUnique();

            entity.HasIndex(e => new { e.TournamentId, e.UserId }, "UQ__Tourname__7D1B9FD62AB46169").IsUnique();

            entity.Property(e => e.JoinedAt)
                .HasDefaultValueSql("(getdate())", "DF__Tournamen__Joine__2EDAF651")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Tournament).WithMany(p => p.TournamentPlayers)
                .HasForeignKey(d => d.TournamentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Tournamen__Tourn__2FCF1A8A");

            entity.HasOne(d => d.User).WithMany(p => p.TournamentPlayers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Tournamen__UserI__30C33EC3");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4CF896EB48");

            entity.HasIndex(e => e.Email, "UQ_Users_Email").IsUnique();

            entity.HasIndex(e => e.PhoneNumber, "UQ_Users_PhoneNumber").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Users__A9D105343662952E").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())", "DF__Users__CreatedAt__5441852A")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.EmailVerified).HasDefaultValue(false, "DF_Users_EmailVerified");
            entity.Property(e => e.FullName).HasMaxLength(150);
            entity.Property(e => e.IsActive).HasDefaultValue(true, "DF__Users__IsActive__534D60F1");
            entity.Property(e => e.LastLoginAt)
                .HasDefaultValueSql("(NULL)", "DF_Users_LastLoginAt")
                .HasColumnType("datetime");
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.PhoneNumber).HasMaxLength(30);
            entity.Property(e => e.PhoneVerified).HasDefaultValue(false, "DF_Users_PhoneVerified");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(NULL)", "DF_Users_UpdatedAt")
                .HasColumnType("datetime");

            entity.HasOne(d => d.City).WithMany(p => p.Users)
                .HasForeignKey(d => d.CityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users_Cities");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserRole__3214EC075BD6836E");

            entity.HasIndex(e => new { e.UserId, e.RoleId }, "UQ_UserRoles_UserId_RoleId").IsUnique();

            entity.HasIndex(e => new { e.UserId, e.RoleId }, "UQ__UserRole__AF2760AC2F9BCE4C").IsUnique();

            entity.HasOne(d => d.Role).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserRoles__RoleI__5CD6CB2B");

            entity.HasOne(d => d.User).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserRoles__UserI__5BE2A6F2");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
