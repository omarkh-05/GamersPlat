using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DTOs.Booking
{
    public class DTO_StartEndTime
    {
        public TimeOnly StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
    }
}
