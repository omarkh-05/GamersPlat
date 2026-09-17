using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Data;

[Index("UserId", "RoleId", Name = "UQ_UserRoles_UserId_RoleId", IsUnique = true)]
[Index("UserId", "RoleId", Name = "UQ__UserRole__AF2760AC2F9BCE4C", IsUnique = true)]
public partial class UserRole
{
    [Key]
    public int Id { get; set; }

    public int UserId { get; set; }

    public int RoleId { get; set; }

    [ForeignKey("RoleId")]
    [InverseProperty("UserRoles")]
    public virtual Role Role { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("UserRoles")]
    public virtual User User { get; set; } = null!;
}
