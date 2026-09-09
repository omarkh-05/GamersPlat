using System;
using System.Collections.Generic;

namespace Data;

public partial class ResourcesType
{
    public int ResourcesTypeId { get; set; }

    public int CenterId { get; set; }

    public byte DeviceId { get; set; }

    public decimal HourlyPrice { get; set; }

    public int TotalQuantity { get; set; }

    public string? RoomType { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Center Center { get; set; } = null!;

    public virtual Device Device { get; set; } = null!;
}
