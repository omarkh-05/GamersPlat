using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Data;

public partial class Session
{
    [Key]
    public int SessionId { get; set; }

    public int CenterId { get; set; }

    public short GameId { get; set; }

    public byte DeviceId { get; set; }

    public DateOnly SessionDate { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public short MaxPlayers { get; set; }

    public int CreatedByUserId { get; set; }

    [StringLength(50)]
    public string SessionStatus { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("CenterId")]
    [InverseProperty("Sessions")]
    public virtual Center Center { get; set; } = null!;

    [ForeignKey("CreatedByUserId")]
    [InverseProperty("Sessions")]
    public virtual User CreatedByUser { get; set; } = null!;

    [ForeignKey("DeviceId")]
    [InverseProperty("Sessions")]
    public virtual Device Device { get; set; } = null!;

    [ForeignKey("GameId")]
    [InverseProperty("Sessions")]
    public virtual Game Game { get; set; } = null!;

    [InverseProperty("Session")]
    public virtual SessionParticipant? SessionParticipant { get; set; }
}
