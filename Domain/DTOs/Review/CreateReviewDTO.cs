using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.DTOs
{
    public class CreateReviewDTO
    {

        [Range(1, int.MaxValue)]
        public int CenterId { get; set; }

        [Range(1, int.MaxValue)]
        public int BookingId { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [StringLength(1000)]
        public string? Comment { get; set; }
    }
}
