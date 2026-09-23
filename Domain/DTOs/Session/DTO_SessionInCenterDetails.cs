using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DTOs.Session
{
    public class DTO_SessionInCenterDetails
    {
        public int SessionId { get; set; }
        public int Joined { get; set; }
        public int MaxPlayers { get; set; }
        public decimal Price { get; set; }
        public DateOnly SessionDate { get; set; }
        public TimeOnly StartTime { get; set; }
    }
}
