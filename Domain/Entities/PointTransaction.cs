using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Data;

public partial class PointTransaction
{
    [Key]
    public int TransactionId { get; set; }

    public int UserId { get; set; }

    public int? CenterId { get; set; }

    public int Points { get; set; }

    [StringLength(50)]
    public string PointType { get; set; } = null!;

    [StringLength(50)]
    public string ReferenceType { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("CenterId")]
    [InverseProperty("PointTransactions")]
    public virtual Center? Center { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("PointTransactions")]
    public virtual User User { get; set; } = null!;
}
