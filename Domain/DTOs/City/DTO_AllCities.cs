using System.ComponentModel.DataAnnotations;

namespace Domain.DTOs.City
{
    public class DTO_AllCities
    {
        [Key]
        public short CityId { get; set; }

        [StringLength(100)]
        public string Name { get; set; } = null!;
    }
}
