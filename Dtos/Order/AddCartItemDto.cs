using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using milktea_server.Enums;

namespace milktea_server.Dtos.Order
{
    public class AddCartItemDto
    {
        [Required]
        public int MilkteaId { get; set; }

        [Required]
        public string Size { get; set; } = MilkteaSize.S.ToString();

        [Required]
        public int Quantity { get; set; } = 1;
        public List<int> Toppings { get; set; } = [];
    }
}
