using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Data;

public partial class Game
{
    [Key]
    public short GameId { get; set; }

    [StringLength(400)]
    public string GameName { get; set; } = null!;

    [InverseProperty("Game")]
    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();

    [InverseProperty("Game")]
    public virtual ICollection<Tournament> Tournaments { get; set; } = new List<Tournament>();
}
