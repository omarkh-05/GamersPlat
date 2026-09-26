using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Data;

public partial class Center
{
    [Key]
    public int CenterId { get; set; }

    public int? OwnerUserId { get; set; }

    [StringLength(150)]
    public string CenterName { get; set; } = null!;

    public short CityId { get; set; }

    [StringLength(255)]
    public string? CenterAddress { get; set; }

    public string? CenterDescription { get; set; }

    [StringLength(50)]
    public string? CenterStatus { get; set; }

    public bool? IsActive { get; set; }

    [StringLength(50)]
    public string? CenterType { get; set; }

    public TimeOnly? OpenTime { get; set; }

    public TimeOnly? CloseTime { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [InverseProperty("Center")]
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    [InverseProperty("Center")]
    public virtual IEnumerable<CenterImage> CenterImage { get; set; } = new List<CenterImage>();

    [InverseProperty("Center")]
    public virtual ICollection<CenterInvitation> CenterInvitations { get; set; } = new List<CenterInvitation>();

    [ForeignKey("CityId")]
    [InverseProperty("Centers")]
    public virtual City City { get; set; } = null!;

    [InverseProperty("Center")]
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    [InverseProperty("Center")]
    public virtual ICollection<Offer> Offers { get; set; } = new List<Offer>();

    [ForeignKey("OwnerUserId")]
    [InverseProperty("Centers")]
    public virtual User? OwnerUser { get; set; }

    [InverseProperty("Center")]
    public virtual ICollection<PointTransaction> PointTransactions { get; set; } = new List<PointTransaction>();

    [InverseProperty("Center")]
    public virtual ICollection<ResourcesType> ResourcesTypes { get; set; } = new List<ResourcesType>();

    [InverseProperty("Center")]
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    [InverseProperty("Center")]
    public virtual ICollection<Service> Services { get; set; } = new List<Service>();

    [InverseProperty("Center")]
    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();

    [InverseProperty("Center")]
    public virtual ICollection<Staff> Staff { get; set; } = new List<Staff>();

    [InverseProperty("Center")]
    public virtual ICollection<StaffRole> StaffRoles { get; set; } = new List<StaffRole>();

    [InverseProperty("Center")]
    public virtual ICollection<Tournament> Tournaments { get; set; } = new List<Tournament>();
}
