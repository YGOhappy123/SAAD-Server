using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using milktea_server.Dtos.Order;
using milktea_server.Dtos.Response;
using milktea_server.Models;
using milktea_server.Queries;

namespace milktea_server.Interfaces.Services
{
    public interface IOrderService
    {
        Task<ServiceResponse<int>> CreateOrder(CreateOrderDto createOrderDto, int customerId, string locale);
        Task<ServiceResponse<List<Order>>> GetCustomerOrders(int customerId, string? sortByJson);
        Task<ServiceResponse<List<Order>>> GetAllOrders(BaseQueryObject queryObject);
        Task<ServiceResponse<Order>> GetOrderDetail(int orderId, int authUserId, string authUserRole);
        Task<ServiceResponse> UpdateOrderStatus(int orderId, UpdateOrderStatusDto updateOrderStatusDto, int staffId);
    }
}
