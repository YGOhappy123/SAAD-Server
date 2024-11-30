using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using milktea_server.Utilities;

namespace milktea_server.Dtos.Product
{
    public class CreateUpdateMilkteaDto
    {
        [Required]
        public string NameVi { get; set; } = string.Empty;

        [Required]
        public string NameEn { get; set; } = string.Empty;

        [Required]
        public string DescriptionVi { get; set; } = string.Empty;

        [Required]
        public string DescriptionEn { get; set; } = string.Empty;
        public string? Image { get; set; }
        public decimal? PriceS { get; set; }
        public decimal? PriceM { get; set; }
        public decimal? PriceL { get; set; }
        public int CategoryId { get; set; }
        public bool IsAvailable { get; set; } = true;
    }
}
