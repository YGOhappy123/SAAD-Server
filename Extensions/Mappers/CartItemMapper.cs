using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using milktea_server.Dtos.Order;
using milktea_server.Models;

namespace milktea_server.Extensions.Mappers
{
    public static class CartItemMapper
    {
        public static CartItemDto ToCartItemDto(this CartItem cartItem)
        {
            return new CartItemDto
            {
                Id = cartItem.Id,
                MilkteaId = cartItem.MilkteaId,
                Size = cartItem.Size.ToString(),
                Quantity = cartItem.Quantity,
                Toppings = cartItem.Toppings.Select(tp => tp.ToppingId).OrderBy(tpId => tpId).ToList(),
            };
        }
    }
}
