using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace milktea_server.Models
{
    public class OrderItemTopping
    {
        public int Id { get; set; }

        [Column(TypeName = "decimal(6,2)")]
        public decimal Price { get; set; }
        public int? ToppingId { get; set; }
        public Topping? Topping { get; set; }
        public int? OrderItemId { get; set; }
        public OrderItem? OrderItem { get; set; }
    }
}
