using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace milktea_server.Dtos.Product
{
    public class MilkteaDto
    {
        public int Id { get; set; }
        public string NameVi { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public string DescriptionVi { get; set; } = string.Empty;
        public string DescriptionEn { get; set; } = string.Empty;
        public string? Image { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public MilkteaPrice? Price { get; set; }
        public CategoryDto? Category { get; set; }
    }

    public class MilkteaPrice
    {
        public decimal? S { get; set; }
        public decimal? M { get; set; }
        public decimal? L { get; set; }
    }
}
