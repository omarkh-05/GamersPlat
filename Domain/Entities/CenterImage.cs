using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Data;

public partial class CenterImage
{
    [Key]
    public int ImageId { get; set; }

    public int CenterId { get; set; }

    [StringLength(500)]
    public string ImageUrl { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    public bool IsMain { get; set; }

    [ForeignKey("CenterId")]
    [InverseProperty("CenterImage")]
    public virtual Center Center { get; set; } = null!;
}
