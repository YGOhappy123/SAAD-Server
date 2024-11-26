using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using milktea_server.Dtos.Order;
using milktea_server.Models;
using milktea_server.Queries;

namespace milktea_server.Interfaces.Repositories
{
    public interface IOrderRepository
    {
        Task CreateOrder(Order newOrder);
        Task<(List<Order>, int)> GetAllOrders(BaseQueryObject queryObject);
        Task<Order?> GetOrderById(int orderId);
        Task<Order?> GetOrderDetailById(int orderId);
        Task<List<Order>> GetCustomerOrders(int customerId, string? sortByJson);
        Task UpdateOrder(Order order);
        Task<int> CountCustomerOrders(int customerId, string timeUnit);
    }
}
