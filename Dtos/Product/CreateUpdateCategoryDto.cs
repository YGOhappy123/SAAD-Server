using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace milktea_server.Dtos.Product
{
    public class CreateUpdateCategoryDto
    {
        [Required]
        public string NameVi { get; set; } = string.Empty;

        [Required]
        public string NameEn { get; set; } = string.Empty;
    }
}
