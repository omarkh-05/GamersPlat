
using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.DTOs.Owner
{
    public class UpdateOfferDTO
    {
        [Required]
        [StringLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [StringLength(50)]
        public string? DiscountType { get; set; }

        public double? DiscountValue { get; set; }

        public short? MaxUses { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public TimeOnly? StartTime { get; set; }

        public TimeOnly? EndTime { get; set; }

        public bool IsActive { get; set; }
    }
}