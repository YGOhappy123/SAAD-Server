using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using milktea_server.Dtos.Order;
using milktea_server.Models;

namespace milktea_server.Extensions.Mappers
{
    public static class OrderMapper
    {
        public static OrderDto ToOrderDto(this Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                TotalPrice = order.TotalPrice,
                Note = order.Note,
                RejectionReason = order.RejectionReason,
                Status = order.Status.ToString(),
                CreatedAt = order.CreatedAt,
                UpdatedAt = order.UpdatedAt,
                Customer =
                    order.Customer != null
                        ? new OrderUserDto
                        {
                            FirstName = order.Customer.FirstName,
                            LastName = order.Customer.LastName,
                            Avatar = order.Customer.Avatar,
                        }
                        : null,
                ProcessingStaff =
                    order.ProcessingStaff != null
                        ? new OrderUserDto
                        {
                            FirstName = order.ProcessingStaff.FirstName,
                            LastName = order.ProcessingStaff.LastName,
                            Avatar = order.ProcessingStaff.Avatar,
                        }
                        : null,
                Items = order
                    .Items.Select(item => new OrderItemDto
                    {
                        NameVi = item.Milktea?.NameVi,
                        NameEn = item.Milktea?.NameEn,
                        Image = item.Milktea?.Image,
                        IsAvailable = item.Milktea?.IsAvailable,

                        MilkteaId = item.MilkteaId,
                        Quantity = item.Quantity,
                        Price = item.Price,
                        Size = item.Size.ToString(),

                        Toppings = item
                            .Toppings.Select(tp => new OrderItemToppingDto
                            {
                                NameVi = tp.Topping?.NameVi,
                                NameEn = tp.Topping?.NameEn,
                                Image = tp.Topping?.Image,
                                IsAvailable = tp.Topping?.IsAvailable,

                                ToppingId = tp.ToppingId,
                                Price = tp.Price,
                            })
                            .ToList(),
                    })
                    .ToList(),
            };
        }
    }
}
