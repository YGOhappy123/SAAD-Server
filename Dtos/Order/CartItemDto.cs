using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using milktea_server.Enums;

namespace milktea_server.Dtos.Order
{
    public class CartItemDto
    {
        public int Id { get; set; }
        public int? CustomerId { get; set; }
        public int? MilkteaId { get; set; }
        public string Size { get; set; } = MilkteaSize.S.ToString();
        public List<int?> Toppings { get; set; } = [];
    }
}
