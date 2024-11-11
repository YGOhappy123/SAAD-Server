using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace milktea_server.Models
{
    public class CartItemTopping
    {
        public int Id { get; set; }
        public int? ToppingId { get; set; }
        public Topping? Topping { get; set; }
        public int? CartItemId { get; set; }
        public CartItem? CartItem { get; set; }
    }
}
