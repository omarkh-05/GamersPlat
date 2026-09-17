using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Data;

[Index("RoleName", Name = "UQ_Roles_RoleName", IsUnique = true)]
[Index("RoleName", Name = "UQ__Roles__8A2B6160890B126E", IsUnique = true)]
public partial class Role
{
    [Key]
    public int RoleId { get; set; }

    [StringLength(50)]
    public string RoleName { get; set; } = null!;

    [InverseProperty("Role")]
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
