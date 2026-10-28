using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Data;

[Index("Name", Name = "UQ_Countries_Name", IsUnique = true)]
[Index("Name", Name = "UQ__Countrie__737584F6BDBF0F11", IsUnique = true)]
public partial class Country
{
    [Key]
    public int CountryId { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = null!;

    [InverseProperty("Country")]
    public virtual ICollection<City> Cities { get; set; } = new List<City>();
}
