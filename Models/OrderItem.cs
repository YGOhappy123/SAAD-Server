using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using milktea_server.Enums;

namespace milktea_server.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int Quantity { get; set; } = 1;
        public MilkteaSize Size { get; set; } = MilkteaSize.S;

        [Column(TypeName = "decimal(6,2)")]
        public decimal Price { get; set; }
        public int? OrderId { get; set; }
        public Order? Order { get; set; }
        public int? MilkteaId { get; set; }
        public Milktea? Milktea { get; set; }
        public List<OrderItemTopping> Toppings { get; set; } = [];
    }
}
