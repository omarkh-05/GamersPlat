
using System.ComponentModel.DataAnnotations;

namespace Domain.DTOs.Owner
{
    public class DTO_CreateResourcesType
    {
        [Range(1, int.MaxValue)]
        public int CenterId { get; set; }

        [Range(1, byte.MaxValue)]
        public byte DeviceId { get; set; }

        [Range(0.01, 99999999.99)]
        public decimal HourlyPrice { get; set; }

        [Range(1, int.MaxValue)]
        public int TotalQuantity { get; set; }

        [StringLength(10)]
        public string? RoomType { get; set; }

        public bool IsActive { get; set; } = true;
    }
}