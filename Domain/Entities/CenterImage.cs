using System;
using System.Collections.Generic;

namespace Data;

public partial class CenterImage
{
    public int ImageId { get; set; }

    public int CenterId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public bool IsMain { get; set; }

    public virtual Center Center { get; set; } = null!;
}
