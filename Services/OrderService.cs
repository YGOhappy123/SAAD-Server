using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using milktea_server.Dtos.Order;
using milktea_server.Dtos.Response;
using milktea_server.Enums;
using milktea_server.Interfaces.Repositories;
using milktea_server.Interfaces.Services;
using milktea_server.Models;
using milktea_server.Queries;
using milktea_server.Utilities;

namespace milktea_server.Services
{
    public class OrderService : IOrderService
    {
        private readonly IConfiguration _configuration;
        private readonly IOrderRepository _orderRepo;
        private readonly IMilkteaRepository _milkteaRepo;
        private readonly IToppingRepository _toppingRepo;
        private readonly ICartRepository _cartRepo;
        private readonly ICustomerRepository _customerRepo;
        private readonly IMailerService _mailerService;

        public OrderService(
            IConfiguration configuration,
            IOrderRepository orderRepo,
            IMilkteaRepository milkteaRepo,
            IToppingRepository toppingRepo,
            ICartRepository cartRepo,
            ICustomerRepository customerRepo,
            IMailerService mailerService
        )
        {
            _configuration = configuration;
            _orderRepo = orderRepo;
            _milkteaRepo = milkteaRepo;
            _toppingRepo = toppingRepo;
            _cartRepo = cartRepo;
            _customerRepo = customerRepo;
            _mailerService = mailerService;
        }

        public async Task<ServiceResponse<int>> CreateOrder(CreateOrderDto createOrderDto, int customerId, string locale)
        {
            try
            {
                var newOrder = new Order
                {
                    CustomerId = customerId,
                    Note = createOrderDto.Note,
                    Status = OrderStatus.Pending,
                    Items = [],
                };

                foreach (var item in createOrderDto.Items)
                {
                    var milktea = await _milkteaRepo.GetMilkteaById(item.MilkteaId);
                    if (milktea == null || !milktea.IsActive || !milktea.IsAvailable)
                    {
                        throw new Exception(ErrorMessage.PRODUCT_NOT_FOUND_OR_UNAVAILABLE);
                    }

                    var milkteaPrice =
                        (
                            item.Size == "S" ? milktea.PriceS
                            : item.Size == "M" ? milktea.PriceM
                            : milktea.PriceL
                        ) ?? 0;

                    var orderItem = new OrderItem
                    {
                        MilkteaId = item.MilkteaId,
                        Size = Enum.Parse<MilkteaSize>(item.Size),
                        Quantity = item.Quantity,
                        Price = milkteaPrice,
                        Toppings = [],
                    };

                    foreach (var toppingId in item.Toppings)
                    {
                        var topping = await _toppingRepo.GetToppingById(toppingId);
                        if (topping == null || !topping.IsActive || !topping.IsAvailable)
                        {
                            throw new Exception(ErrorMessage.PRODUCT_NOT_FOUND_OR_UNAVAILABLE);
                        }

                        var orderItemTopping = new OrderItemTopping { ToppingId = toppingId, Price = topping.Price };
                        orderItem.Toppings.Add(orderItemTopping);
                    }

                    newOrder.Items.Add(orderItem);
                }

                newOrder.TotalPrice = newOrder.Items.Sum(item => item.Quantity * (item.Price + item.Toppings.Sum(tp => tp.Price)));

                await _orderRepo.CreateOrder(newOrder);
                await _cartRepo.ResetCart(customerId);

                var customer = await _customerRepo.GetCustomerById(customerId);
                if (!string.IsNullOrEmpty(customer?.Email))
                {
                    await _mailerService.SendOrderConfirmationEmail(
                        customer.Email,
                        $"{customer.LastName} {customer.FirstName}",
                        newOrder.Id.ToString(),
                        $"{_configuration["Application:ClientUrl"]}/profile/orders",
                        newOrder.CreatedAt.ToString("HH:mm dd/MM/yyyy"),
                        locale
                    );
                }

                return new ServiceResponse<int>
                {
                    Status = ResStatusCode.CREATED,
                    Success = true,
                    Message = SuccessMessage.CREATE_ORDER_SUCCESSFULLY,
                    Data = newOrder.Id,
                };
            }
            catch (Exception ex)
            {
                return new ServiceResponse<int>
                {
                    Status = ResStatusCode.BAD_REQUEST,
                    Success = false,
                    Message = ex.Message ?? ErrorMessage.CREATE_ORDER_FAILED,
                };
            }
        }

        public async Task<ServiceResponse<List<Order>>> GetAllOrders(BaseQueryObject queryObject)
        {
            var (orders, total) = await _orderRepo.GetAllOrders(queryObject);

            return new ServiceResponse<List<Order>>
            {
                Status = ResStatusCode.OK,
                Success = true,
                Data = orders,
                Total = total,
                Took = orders.Count,
            };
        }

        public async Task<ServiceResponse<List<Order>>> GetCustomerOrders(int customerId, string? sortByJson)
        {
            var orders = await _orderRepo.GetCustomerOrders(customerId, sortByJson);

            return new ServiceResponse<List<Order>>
            {
                Status = ResStatusCode.OK,
                Success = true,
                Data = orders,
            };
        }

        public async Task<ServiceResponse<Order>> GetOrderDetail(int orderId, int authUserId, string authUserRole)
        {
            var order = await _orderRepo.GetOrderDetailById(orderId);

            if (order == null)
            {
                return new ServiceResponse<Order>
                {
                    Status = ResStatusCode.NOT_FOUND,
                    Success = false,
                    Message = ErrorMessage.ORDER_NOT_FOUND,
                };
            }

            if (authUserRole == UserRole.Customer.ToString() && authUserId != order.CustomerId)
            {
                return new ServiceResponse<Order>
                {
                    Status = ResStatusCode.FORBIDDEN,
                    Success = false,
                    Message = ErrorMessage.NO_PERMISSION,
                };
            }

            return new ServiceResponse<Order>
            {
                Status = ResStatusCode.OK,
                Success = true,
                Data = order,
            };
        }

        public async Task<ServiceResponse> UpdateOrderStatus(int orderId, UpdateOrderStatusDto updateOrderStatusDto, int staffId)
        {
            var order = await _orderRepo.GetOrderById(orderId);

            if (order == null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.NOT_FOUND,
                    Success = false,
                    Message = ErrorMessage.ORDER_NOT_FOUND,
                };
            }

            if (order.Status != OrderStatus.Pending && order.ProcessingStaffId != staffId)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.FORBIDDEN,
                    Success = false,
                    Message = ErrorMessage.NO_PERMISSION,
                };
            }

            order.ProcessingStaffId = staffId;
            order.Status = Enum.Parse<OrderStatus>(updateOrderStatusDto.Status);
            order.RejectionReason = updateOrderStatusDto.RejectionReason;
            order.UpdatedAt = TimestampHandler.GetNow();

            await _orderRepo.UpdateOrder(order);

            return new ServiceResponse
            {
                Status = ResStatusCode.OK,
                Success = true,
                Message = SuccessMessage.UPDATE_ORDER_STATUS_SUCCESSFULLY,
            };
        }
    }
}
