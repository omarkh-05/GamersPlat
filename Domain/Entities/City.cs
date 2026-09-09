using System;
using System.Collections.Generic;

namespace Data;

public partial class City
{
    public short CityId { get; set; }

    public string Name { get; set; } = null!;

    public int CountryId { get; set; }

    public virtual ICollection<Center> Centers { get; set; } = new List<Center>();

    public virtual Country Country { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
