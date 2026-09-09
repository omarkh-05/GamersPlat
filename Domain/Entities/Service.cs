using System;
using System.Collections.Generic;

namespace Data;

public partial class Service
{
    public byte ServiceId { get; set; }

    public int CenterId { get; set; }

    public string Name { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Center Center { get; set; } = null!;
}
