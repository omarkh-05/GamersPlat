using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Data;

[Index(nameof(ResourcesTypeId), nameof(Label), Name = "UQ_ResourceUnits_ResourcesTypeId_Label", IsUnique = true)]
public partial class ResourceUnit
{
    [Key]
    public int ResourceUnitId { get; set; }

    public int ResourcesTypeId { get; set; }

    [StringLength(100)]
    public string Label { get; set; } = string.Empty;

    [StringLength(20)]
    public string Status { get; set; } = "Available";

    [ForeignKey(nameof(ResourcesTypeId))]
    public virtual ResourcesType ResourcesType { get; set; } = null!;
}
