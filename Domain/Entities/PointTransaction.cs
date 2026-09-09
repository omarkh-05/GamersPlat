using System;
using System.Collections.Generic;

namespace Data;

public partial class PointTransaction
{
    public int TransactionId { get; set; }

    public int UserId { get; set; }

    public int? CenterId { get; set; }

    public int Points { get; set; }

    public string PointType { get; set; } = null!;

    public string ReferenceType { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual Center? Center { get; set; }

    public virtual User User { get; set; } = null!;
}
