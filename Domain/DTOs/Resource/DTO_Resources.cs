using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.DTOs.Resource
{
    public class DTO_Resources
    {
        public int ResourcesTypeId { get; set; }
        public string ResourceName { get; set; } = null!;
    }
}
