using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Data;

public partial class Device
{
    [Key]
    public byte DeviceId { get; set; }

    [StringLength(50)]
    public string DeviceName { get; set; } = null!;

    [InverseProperty("Device")]
    public virtual ICollection<ResourcesType> ResourcesTypes { get; set; } = new List<ResourcesType>();

    [InverseProperty("Device")]
    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();

    [InverseProperty("Device")]
    public virtual ICollection<Tournament> Tournaments { get; set; } = new List<Tournament>();
}
