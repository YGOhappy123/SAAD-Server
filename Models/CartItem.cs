using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using milktea_server.Enums;

namespace milktea_server.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public int Quantity { get; set; } = 1;
        public MilkteaSize Size { get; set; } = MilkteaSize.S;
        public int? CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public int? MilkteaId { get; set; }
        public Milktea? Milktea { get; set; }
        public List<CartItemTopping> Toppings { get; set; } = [];
    }
}
