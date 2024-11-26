using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using milktea_server.Models;

namespace milktea_server.Interfaces.Repositories
{
    public interface ICartRepository
    {
        Task<CartItem?> GetExactCartItemFromCustomer(int customerId, int milkteaId, string size, List<int> toppings);
        Task<CartItem?> GetCartItemById(int cartItemId);
        Task<List<CartItem>> GetCustomerCartItems(int customerId);
        Task AddCartItem(CartItem cartItem);
        Task UpdateCartItem(CartItem cartItem);
        Task RemoveCartItem(CartItem cartItem);
        Task ResetCart(int customerId);
    }
}
