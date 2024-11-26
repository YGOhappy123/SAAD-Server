using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using milktea_server.Dtos.Order;
using milktea_server.Dtos.Response;
using milktea_server.Dtos.Statistic;
using milktea_server.Enums;
using milktea_server.Extensions.Mappers;
using milktea_server.Interfaces.Repositories;
using milktea_server.Interfaces.Services;
using milktea_server.Models;
using milktea_server.Queries;
using milktea_server.Utilities;

namespace milktea_server.Services
{
    public class UserService : IUserService
    {
        private readonly ICartRepository _cartRepo;
        private readonly IOrderRepository _orderRepo;
        private readonly ICustomerRepository _customerRepo;
        private readonly IAdminRepository _adminRepo;

        public UserService(
            ICartRepository cartRepo,
            IOrderRepository orderRepo,
            ICustomerRepository customerRepo,
            IAdminRepository adminRepo
        )
        {
            _cartRepo = cartRepo;
            _orderRepo = orderRepo;
            _customerRepo = customerRepo;
            _adminRepo = adminRepo;
        }

        public async Task<ServiceResponse<List<CustomerWithSales>>> GetAllCustomers(BaseQueryObject queryObject)
        {
            var (customers, total) = await _customerRepo.GetAllCustomers(queryObject);

            List<CustomerWithSales> customersWithSales = [];
            foreach (var cus in customers)
            {
                var totalOrdersThisMonth = await _orderRepo.CountCustomerOrders(cus.Id, "monthly");
                var totalOrdersThisYear = await _orderRepo.CountCustomerOrders(cus.Id, "yearly");

                customersWithSales.Add(
                    new CustomerWithSales
                    {
                        Id = cus.Id,
                        FirstName = cus.FirstName,
                        LastName = cus.LastName,
                        Email = cus.Email,
                        Avatar = cus.Avatar,
                        CreatedAt = cus.CreatedAt,
                        PhoneNumber = cus.PhoneNumber,
                        Address = cus.Address,
                        TotalOrdersThisMonth = totalOrdersThisMonth,
                        TotalOrdersThisYear = totalOrdersThisYear,
                    }
                );
            }

            return new ServiceResponse<List<CustomerWithSales>>
            {
                Status = ResStatusCode.OK,
                Success = true,
                Data = customersWithSales,
                Total = total,
                Took = customers.Count,
            };
        }

        public async Task<ServiceResponse> UpdateCustomerProfile()
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<List<Admin>>> GetAllAdmins(BaseQueryObject queryObject)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse> UpdateAdminProfile()
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<List<CartItem>>> GetCustomerCartItems(int customerId)
        {
            var cartItems = await _cartRepo.GetCustomerCartItems(customerId);

            return new ServiceResponse<List<CartItem>>
            {
                Status = ResStatusCode.OK,
                Success = true,
                Data = cartItems,
            };
        }

        public async Task<ServiceResponse> AddCartItem(AddCartItemDto addCartItemDto, int customerId)
        {
            var exactCartItem = await _cartRepo.GetExactCartItemFromCustomer(
                customerId,
                addCartItemDto.MilkteaId,
                addCartItemDto.Size,
                addCartItemDto.Toppings
            );

            if (exactCartItem == null)
            {
                var newCartItem = new CartItem
                {
                    CustomerId = customerId,
                    MilkteaId = addCartItemDto.MilkteaId,
                    Size = Enum.Parse<MilkteaSize>(addCartItemDto.Size),
                    Quantity = addCartItemDto.Quantity,
                    Toppings = [],
                };

                foreach (var toppingId in addCartItemDto.Toppings)
                {
                    var cartItemTopping = new CartItemTopping { ToppingId = toppingId };
                    newCartItem.Toppings.Add(cartItemTopping);
                }

                await _cartRepo.AddCartItem(newCartItem);
            }
            else
            {
                exactCartItem.Quantity += addCartItemDto.Quantity;
                await _cartRepo.UpdateCartItem(exactCartItem);
            }

            return new ServiceResponse
            {
                Status = exactCartItem == null ? ResStatusCode.CREATED : ResStatusCode.OK,
                Success = true,
                Message = SuccessMessage.ADD_TO_CART_SUCCESSFULLY,
            };
        }

        public async Task<ServiceResponse> UpdateCartItem(UpdateCartItemDto updateCartItemDto, int cartItemId, int customerId)
        {
            var targetCartItem = await _cartRepo.GetCartItemById(cartItemId);
            if (targetCartItem == null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.NOT_FOUND,
                    Success = false,
                    Message = ErrorMessage.CART_ITEM_NOT_FOUND,
                };
            }

            if (targetCartItem.CustomerId != customerId)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.FORBIDDEN,
                    Success = false,
                    Message = ErrorMessage.NO_PERMISSION,
                };
            }

            if (updateCartItemDto.Type == "increase")
            {
                targetCartItem.Quantity += updateCartItemDto.Quantity;
            }
            else
            {
                targetCartItem.Quantity -= updateCartItemDto.Quantity;
            }

            await _cartRepo.UpdateCartItem(targetCartItem);

            return new ServiceResponse
            {
                Status = ResStatusCode.OK,
                Success = true,
                Message = SuccessMessage.UPDATE_CART_SUCCESSFULLY,
            };
        }

        public async Task<ServiceResponse> RemoveCartItem(int cartItemId, int customerId)
        {
            var targetCartItem = await _cartRepo.GetCartItemById(cartItemId);
            if (targetCartItem == null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.NOT_FOUND,
                    Success = false,
                    Message = ErrorMessage.CART_ITEM_NOT_FOUND,
                };
            }

            if (targetCartItem.CustomerId != customerId)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.FORBIDDEN,
                    Success = false,
                    Message = ErrorMessage.NO_PERMISSION,
                };
            }

            await _cartRepo.RemoveCartItem(targetCartItem);

            return new ServiceResponse
            {
                Status = ResStatusCode.OK,
                Success = true,
                Message = SuccessMessage.REMOVE_FROM_CART_SUCCESSFULLY,
            };
        }

        public async Task<ServiceResponse> ResetCustomerCart(int customerId)
        {
            await _cartRepo.ResetCart(customerId);

            return new ServiceResponse
            {
                Status = ResStatusCode.OK,
                Success = true,
                Message = SuccessMessage.RESET_CART_SUCCESSFULLY,
            };
        }
    }
}
