using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace milktea_server.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string NameVi { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsActive { get; set; }
    }
}
