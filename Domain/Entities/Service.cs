using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Data;

[Index("Name", Name = "UQ_Services_Name", IsUnique = true)]
public partial class Service
{
    [Key]
    public byte ServiceId { get; set; }

    [StringLength(50)]
    public string Name { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [InverseProperty("Service")]
    public virtual ICollection<CenterService> CenterServices { get; set; } = new List<CenterService>();
}
