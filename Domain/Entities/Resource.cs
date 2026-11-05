using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Data;

public partial class Resource
{
    [Key]
    public int ResourcesTypeId { get; set; }

    [StringLength(100)]
    public string ResourceName { get; set; } = null!;

    [InverseProperty("ResourcesType")]
    public virtual ICollection<CenterResource> CenterResources { get; set; } = new List<CenterResource>();
}
