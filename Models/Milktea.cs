using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace milktea_server.Models
{
    public class Milktea : Product
    {
        [Column(TypeName = "decimal(10,2)")]
        public decimal? PriceS { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? PriceM { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? PriceL { get; set; }
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
        public List<OrderItem> OrderItems { get; set; } = [];
    }
}
