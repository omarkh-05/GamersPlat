using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Data;

[Index("CountryId", "Name", Name = "UQ_Cities_CountryId_Name", IsUnique = true)]
[Index("Name", "CountryId", Name = "UQ__Cities__696C4FEC80312ADA", IsUnique = true)]
public partial class City
{
    [Key]
    public short CityId { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = null!;

    public int CountryId { get; set; }

    [InverseProperty("City")]
    public virtual ICollection<Center> Centers { get; set; } = new List<Center>();

    [ForeignKey("CountryId")]
    [InverseProperty("Cities")]
    public virtual Country Country { get; set; } = null!;

    [InverseProperty("City")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
