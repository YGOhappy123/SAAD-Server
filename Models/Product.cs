using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace milktea_server.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string NameVi { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public string DescriptionVi { get; set; } = string.Empty;
        public string DescriptionEn { get; set; } = string.Empty;
        public string? Image { get; set; }
        public bool IsAvailable { get; set; } = false;
        public bool IsActive { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
