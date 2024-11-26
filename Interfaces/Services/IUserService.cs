using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using milktea_server.Dtos.Order;
using milktea_server.Dtos.Response;
using milktea_server.Dtos.Statistic;
using milktea_server.Models;
using milktea_server.Queries;

namespace milktea_server.Interfaces.Services
{
    public interface IUserService
    {
        Task<ServiceResponse<List<CustomerWithSales>>> GetAllCustomers(BaseQueryObject queryObject);
        Task<ServiceResponse> UpdateCustomerProfile();

        Task<ServiceResponse<List<Admin>>> GetAllAdmins(BaseQueryObject queryObject);
        Task<ServiceResponse> UpdateAdminProfile();

        Task<ServiceResponse<List<CartItem>>> GetCustomerCartItems(int customerId);
        Task<ServiceResponse> AddCartItem(AddCartItemDto addCartItemDto, int customerId);
        Task<ServiceResponse> UpdateCartItem(UpdateCartItemDto updateCartItemDto, int cartItemId, int customerId);
        Task<ServiceResponse> RemoveCartItem(int cartItemId, int customerId);
        Task<ServiceResponse> ResetCustomerCart(int customerId);
    }
}
