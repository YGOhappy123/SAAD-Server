using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using milktea_server.Enums;

namespace milktea_server.Dtos.Order
{
    public class OrderDto
    {
        public int Id { get; set; }
        public decimal TotalPrice { get; set; }
        public string? Note { get; set; }
        public string? RejectionReason { get; set; }
        public string Status { get; set; } = OrderStatus.Pending.ToString();
        public int? ProcessingStaffId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public OrderUserDto? Customer { get; set; }
        public OrderUserDto? ProcessingStaff { get; set; }
        public List<OrderItemDto> Items { get; set; } = [];
    }

    public class OrderUserDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Avatar { get; set; }
    }

    public class OrderProductDto
    {
        public decimal Price { get; set; }
        public string? NameVi { get; set; } = string.Empty;
        public string? NameEn { get; set; } = string.Empty;
        public string? Image { get; set; }
        public bool? IsAvailable { get; set; } = false;
    }

    public class OrderItemDto : OrderProductDto
    {
        public int Quantity { get; set; } = 1;
        public int? MilkteaId { get; set; }
        public string Size { get; set; } = MilkteaSize.S.ToString();

        public List<OrderItemToppingDto> Toppings { get; set; } = [];
    }

    public class OrderItemToppingDto : OrderProductDto
    {
        public int? ToppingId { get; set; }
    }
}
