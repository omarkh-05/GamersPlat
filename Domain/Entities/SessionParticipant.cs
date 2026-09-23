using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Data;

[Index("SessionId", "UserId", Name = "UQ_SessionParticipants_SessionId_UserId", IsUnique = true)]
public partial class SessionParticipant
{
    [Key]
    public int Id { get; set; }

    public int SessionId { get; set; }

    public int UserId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime JoinedAt { get; set; }

    [ForeignKey("SessionId")]
    [InverseProperty("SessionParticipants")]
    public virtual Session Session { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("SessionParticipants")]
    public virtual User User { get; set; } = null!;
}
