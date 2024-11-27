using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using milktea_server.Data;
using milktea_server.Enums;
using milktea_server.Interfaces.Repositories;
using milktea_server.Models;
using milktea_server.Queries;
using milktea_server.Utilities;

namespace milktea_server.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDBContext _dbContext;

        public OrderRepository(ApplicationDBContext context)
        {
            _dbContext = context;
        }

        private IQueryable<Order> ApplyFilters(IQueryable<Order> query, Dictionary<string, object> filters)
        {
            foreach (var filter in filters)
            {
                string value = filter.Value.ToString() ?? "";

                if (!string.IsNullOrWhiteSpace(value))
                {
                    switch (filter.Key)
                    {
                        case "startTime":
                            query = query.Where(od => od.CreatedAt >= DateTime.Parse(value));
                            break;
                        case "endTime":
                            query = query.Where(od => od.CreatedAt <= DateTime.Parse(value));
                            break;
                        case "minTotal":
                            query = query.Where(od => od.TotalPrice >= Convert.ToDecimal(value));
                            break;
                        case "maxTotal":
                            query = query.Where(od => od.TotalPrice <= Convert.ToDecimal(value));
                            break;
                        case "customerName":
                            query = query.Where(od => od.Customer!.FirstName.Contains(value) || od.Customer!.LastName.Contains(value));
                            break;
                        default:
                            query = query.Where(od => EF.Property<string>(od, filter.Key.CapitalizeEachWords()) == value);
                            break;
                    }
                }
            }

            return query;
        }

        private IQueryable<Order> ApplySorting(IQueryable<Order> query, Dictionary<string, string> sort)
        {
            foreach (var order in sort)
            {
                query =
                    order.Value == "ASC"
                        ? query.OrderBy(od => EF.Property<object>(od, order.Key.CapitalizeEachWords()))
                        : query.OrderByDescending(od => EF.Property<object>(od, order.Key.CapitalizeEachWords()));
            }

            return query;
        }

        public async Task CreateOrder(Order newOrder)
        {
            await _dbContext.Orders.AddAsync(newOrder);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<(List<Order>, int)> GetAllOrders(BaseQueryObject queryObject)
        {
            var query = _dbContext
                .Orders.Include(od => od.Customer)
                .Include(od => od.ProcessingStaff)
                .Include(od => od.Items)
                .ThenInclude(oi => oi.Milktea)
                .Include(od => od.Items)
                .ThenInclude(oi => oi.Toppings)
                .ThenInclude(oit => oit.Topping)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(queryObject.Filter))
            {
                var parsedFilter = JsonSerializer.Deserialize<Dictionary<string, object>>(queryObject.Filter);
                query = ApplyFilters(query, parsedFilter!);
            }

            if (!string.IsNullOrWhiteSpace(queryObject.Sort))
            {
                var parsedSort = JsonSerializer.Deserialize<Dictionary<string, string>>(queryObject.Sort);
                query = ApplySorting(query, parsedSort!);
            }

            var total = await query.CountAsync();

            if (queryObject.Skip.HasValue)
                query = query.Skip(queryObject.Skip.Value);

            if (queryObject.Limit.HasValue)
                query = query.Take(queryObject.Limit.Value);

            var orders = await query.ToListAsync();

            return (orders, total);
        }

        public async Task<Order?> GetOrderById(int orderId)
        {
            return await _dbContext.Orders.Where(od => od.Id == orderId).FirstOrDefaultAsync();
        }

        public async Task<Order?> GetOrderDetailById(int orderId)
        {
            return await _dbContext
                .Orders.Include(od => od.Customer)
                .Include(od => od.ProcessingStaff)
                .Include(od => od.Items)
                .ThenInclude(oi => oi.Milktea)
                .Include(od => od.Items)
                .ThenInclude(oi => oi.Toppings)
                .ThenInclude(oit => oit.Topping)
                .Where(od => od.Id == orderId)
                .FirstOrDefaultAsync();
        }

        public async Task<List<Order>> GetCustomerOrders(int customerId, string? sortByJson)
        {
            var query = _dbContext
                .Orders.Include(od => od.Customer)
                .Include(od => od.ProcessingStaff)
                .Include(od => od.Items)
                .ThenInclude(oi => oi.Milktea)
                .Include(od => od.Items)
                .ThenInclude(oi => oi.Toppings)
                .ThenInclude(oit => oit.Topping)
                .Where(od => od.CustomerId == customerId)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(sortByJson))
            {
                var parsedSort = JsonSerializer.Deserialize<Dictionary<string, string>>(sortByJson);
                query = ApplySorting(query, parsedSort!);
            }

            return await query.ToListAsync();
        }

        public async Task UpdateOrder(Order order)
        {
            _dbContext.Orders.Update(order);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<int> CountCustomerOrders(int customerId, string timeUnit)
        {
            var currentTime = TimestampHandler.GetNow();
            var startTime = TimestampHandler.GetStartOfTimeByType(currentTime, timeUnit);
            var endTime = TimestampHandler.GetEndOfTimeByType(currentTime, timeUnit);

            return await _dbContext
                .Orders.Where(od =>
                    od.CustomerId == customerId && od.CreatedAt >= startTime && od.CreatedAt <= endTime && od.Status == OrderStatus.Done
                )
                .CountAsync();
        }

        public async Task RejectAllOrdersProcessedByStaff(int staffId)
        {
            var orders = await _dbContext
                .Orders.Where(od => od.Status == OrderStatus.Accepted && od.ProcessingStaffId == staffId)
                .ToListAsync();

            foreach (var order in orders)
            {
                order.Status = OrderStatus.Rejected;
                order.RejectionReason = "Nhân viên này không còn làm việc tại PMT";
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
