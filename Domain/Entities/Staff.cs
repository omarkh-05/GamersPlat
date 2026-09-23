using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Data;

[Index("UserId", Name = "UQ_Staff_User", IsUnique = true)]
public partial class Staff
{
    [Key]
    public int StaffId { get; set; }

    public int UserId { get; set; }

    public int CenterId { get; set; }

    public int StaffRoleId { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsActive { get; set; }

    [ForeignKey("CenterId")]
    [InverseProperty("Staff")]
    public virtual Center Center { get; set; } = null!;

    [ForeignKey("StaffRoleId")]
    [InverseProperty("Staff")]
    public virtual StaffRole StaffRole { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("Staff")]
    public virtual User User { get; set; } = null!;
}
