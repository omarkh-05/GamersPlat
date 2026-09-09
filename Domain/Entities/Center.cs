using System;
using System.Collections.Generic;

namespace Data;

public partial class Center
{
    public int CenterId { get; set; }

    // null اشارة ؟ معناها انه ممكن يكون 
    public int? OwnerUserId { get; set; }

    // null!; ولكن مش محددين قيمة اولية (not null in database) null معناها انه ممنوع يكون
    public string CenterName { get; set; } = null!;

    public short CityId { get; set; }

    public string? CenterAddress { get; set; }

    public string? CenterDescription { get; set; }

    public string? CenterStatus { get; set; }

    public bool? IsActive { get; set; }

    public string? CenterType { get; set; }

    public TimeOnly? OpenTime { get; set; }

    public TimeOnly? CloseTime { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual CenterImage? CenterImage { get; set; }

    public virtual ICollection<CenterInvitation> CenterInvitations { get; set; } = new List<CenterInvitation>();

    public virtual City City { get; set; } = null!;

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Offer> Offers { get; set; } = new List<Offer>();

    public virtual User? OwnerUser { get; set; }

    public virtual ICollection<PointTransaction> PointTransactions { get; set; } = new List<PointTransaction>();

    public virtual ICollection<ResourcesType> ResourcesTypes { get; set; } = new List<ResourcesType>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual ICollection<Service> Services { get; set; } = new List<Service>();

    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();

    public virtual ICollection<Tournament> Tournaments { get; set; } = new List<Tournament>();
}
