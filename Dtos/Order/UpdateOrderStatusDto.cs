using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using milktea_server.Enums;

namespace milktea_server.Dtos.Order
{
    public class UpdateOrderStatusDto
    {
        [Required]
        public string Status { get; set; } = OrderStatus.Accepted.ToString();
        public string? RejectionReason { get; set; }
    }
}
