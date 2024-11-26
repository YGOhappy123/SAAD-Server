using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace milktea_server.Dtos.Order
{
    public class UpdateCartItemDto
    {
        [Required]
        public string Type { get; set; } = "increase";

        [Required]
        public int Quantity { get; set; } = 1;
    }
}
