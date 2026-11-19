using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Data;

[Index("CenterPhone", Name = "UQ_Centers_CenterPhone", IsUnique = true)]
[Index(nameof(Slug), Name = "UQ_Centers_Slug", IsUnique = true)]
public partial class Center
{
    [Key]
    public int CenterId { get; set; }

    public int OwnerUserId { get; set; }

    [StringLength(150)]
    public string CenterName { get; set; } = null!;

    [StringLength(255)]
    public string Slug { get; set; } = null!;

    [StringLength(255)]
    public string CenterAddress { get; set; } = null!;

    public string CenterDescription { get; set; } = null!;

    [StringLength(50)]
    public string CenterStatus { get; set; } = null!;

    [StringLength(20)]
    public string CenterPhone { get; set; } = null!;

    [StringLength(50)]
    public string CenterType { get; set; } = null!;

    public TimeOnly OpenTime { get; set; }

    public TimeOnly? CloseTime { get; set; }

    public short CityId { get; set; }

    public bool IsApproved { get; set; }

    public bool IsActive { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    public int CreatedBy { get; set; }

    [InverseProperty("Center")]
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    [InverseProperty("Center")]
    public virtual ICollection<CenterImage> CenterImages { get; set; } = new List<CenterImage>();

    [InverseProperty("Center")]
    public virtual ICollection<CenterInvitation> CenterInvitations { get; set; } = new List<CenterInvitation>();

    [InverseProperty("Center")]
    public virtual ICollection<CenterResource> CenterResources { get; set; } = new List<CenterResource>();

    [InverseProperty("Center")]
    public virtual ICollection<CenterService> CenterServices { get; set; } = new List<CenterService>();

    [ForeignKey("CityId")]
    [InverseProperty("Centers")]
    public virtual City City { get; set; } = null!;

    [ForeignKey("CreatedBy")]
    [InverseProperty("CenterCreatedByNavigations")]
    public virtual User CreatedByNavigation { get; set; } = null!;

    [InverseProperty("Center")]
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    [InverseProperty("Center")]
    public virtual ICollection<Offer> Offers { get; set; } = new List<Offer>();

    [ForeignKey("OwnerUserId")]
    [InverseProperty("CenterOwnerUsers")]
    public virtual User OwnerUser { get; set; } = null!;

    [InverseProperty("Center")]
    public virtual ICollection<PointTransaction> PointTransactions { get; set; } = new List<PointTransaction>();

    [InverseProperty("Center")]
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    [InverseProperty("Center")]
    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();

    [InverseProperty("Center")]
    public virtual ICollection<Staff> Staff { get; set; } = new List<Staff>();

    [InverseProperty("Center")]
    public virtual ICollection<Tournament> Tournaments { get; set; } = new List<Tournament>();
}
