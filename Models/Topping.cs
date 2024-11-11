using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace milktea_server.Models
{
    public class Topping : Product
    {
        [Column(TypeName = "decimal(6,2)")]
        public decimal Price { get; set; }
    }
}
