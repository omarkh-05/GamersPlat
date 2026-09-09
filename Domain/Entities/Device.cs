using System;
using System.Collections.Generic;

namespace Data;

public partial class Device
{
    public byte DeviceId { get; set; }

    public string DeviceName { get; set; } = null!;

    public virtual ICollection<ResourcesType> ResourcesTypes { get; set; } = new List<ResourcesType>();

    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();

    public virtual ICollection<Tournament> Tournaments { get; set; } = new List<Tournament>();
}
