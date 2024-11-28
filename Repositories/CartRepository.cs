using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using milktea_server.Data;
using milktea_server.Enums;
using milktea_server.Interfaces.Repositories;
using milktea_server.Models;

namespace milktea_server.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDBContext _dbContext;

        public CartRepository(ApplicationDBContext context)
        {
            _dbContext = context;
        }

        public async Task<CartItem?> GetExactCartItemFromCustomer(int customerId, int milkteaId, string size, List<int> toppings)
        {
            var matchingMilkteas = await _dbContext
                .CartItems.Include(ci => ci.Toppings)
                .Where(ci => ci.CustomerId == customerId && ci.MilkteaId == milkteaId && ci.Size == Enum.Parse<MilkteaSize>(size))
                .ToListAsync();

            if (matchingMilkteas.Count == 0)
            {
                return null;
            }

            var sortedToppingIds = toppings.OrderBy(id => id);

            foreach (var item in matchingMilkteas)
            {
                if (item.Toppings.Select(cit => (int)cit.ToppingId!).SequenceEqual(sortedToppingIds))
                {
                    return item;
                }
            }

            return null;
        }

        public async Task<CartItem?> GetCartItemById(int cartItemId)
        {
            return await _dbContext.CartItems.Where(ci => ci.Id == cartItemId).FirstOrDefaultAsync();
        }

        public async Task<List<CartItem>> GetCustomerCartItems(int customerId)
        {
            return await _dbContext.CartItems.Include(ci => ci.Toppings).Where(ci => ci.CustomerId == customerId).ToListAsync();
        }

        public async Task AddCartItem(CartItem cartItem)
        {
            _dbContext.CartItems.Add(cartItem);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateCartItem(CartItem cartItem)
        {
            _dbContext.CartItems.Update(cartItem);
            await _dbContext.SaveChangesAsync();
        }

        public async Task RemoveCartItem(CartItem cartItem)
        {
            _dbContext.CartItems.Remove(cartItem);
            await _dbContext.SaveChangesAsync();
        }

        public async Task ResetCart(int customerId)
        {
            var cartItems = await _dbContext.CartItems.Where(ci => ci.CustomerId == customerId).ToListAsync();

            _dbContext.CartItems.RemoveRange(cartItems);
            await _dbContext.SaveChangesAsync();
        }

        public async Task RemoveCartItemsRelatedToCategory(int categoryId)
        {
            var cartItems = await _dbContext
                .CartItems.Include(ci => ci.Milktea)
                .Where(ci => ci.Milktea != null && ci.Milktea.CategoryId == categoryId)
                .ToListAsync();

            _dbContext.CartItems.RemoveRange(cartItems);
            await _dbContext.SaveChangesAsync();
        }

        public async Task RemoveCartItemsRelatedToMilktea(int milkteaId)
        {
            var cartItems = await _dbContext.CartItems.Where(ci => ci.MilkteaId == milkteaId).ToListAsync();

            _dbContext.CartItems.RemoveRange(cartItems);
            await _dbContext.SaveChangesAsync();
        }

        public async Task RemoveCartItemsRelatedToTopping(int toppingId)
        {
            var cartItemIds = await _dbContext
                .CartItemToppings.Where(cit => cit.ToppingId == toppingId)
                .Select(cit => cit.CartItemId)
                .ToListAsync();

            if (cartItemIds.Count != 0)
            {
                var cartItems = await _dbContext.CartItems.Where(ci => cartItemIds.Contains(ci.Id)).ToListAsync();

                _dbContext.CartItems.RemoveRange(cartItems);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
