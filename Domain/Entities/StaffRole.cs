using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Data;

[Index("Name", Name = "UQ_StaffRoles_Name", IsUnique = true)]
public partial class StaffRole
{
    [Key]
    public int StaffRoleId { get; set; }

    public int CenterId { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = null!;

    public int Permissions { get; set; }

    [ForeignKey("CenterId")]
    [InverseProperty("StaffRoles")]
    public virtual Center Center { get; set; } = null!;

    [InverseProperty("StaffRole")]
    public virtual ICollection<Staff> Staff { get; set; } = new List<Staff>();
}
