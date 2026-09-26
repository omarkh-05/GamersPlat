using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Data;

public partial class Service
{
    [Key]
    public byte ServiceId { get; set; }

    public int CenterId { get; set; }

    [StringLength(50)]
    public string Name { get; set; } = null!;

    public bool IsActive { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("CenterId")]
    [InverseProperty("Services")]
    public virtual Center Center { get; set; } = null!;
}
