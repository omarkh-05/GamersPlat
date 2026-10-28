using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Data;

[Index(nameof(CenterId), nameof(Status), nameof(CreatedAt))]
public partial class CustomerRequest
{
    [Key]
    public int RequestId { get; set; }

    public int CenterId { get; set; }

    public int? PlayerId { get; set; }

    [StringLength(100)]
    public string Type { get; set; } = string.Empty;

    [StringLength(150)]
    public string PlayerName { get; set; } = string.Empty;

    [StringLength(200)]
    public string Subject { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;

    [StringLength(50)]
    public string Status { get; set; } = "Pending";

    [Column(TypeName = "datetime2")]
    public DateTime CreatedAt { get; set; }

    [ForeignKey(nameof(CenterId))]
    public virtual Center Center { get; set; } = null!;

    [ForeignKey(nameof(PlayerId))]
    public virtual User? Player { get; set; }
}
