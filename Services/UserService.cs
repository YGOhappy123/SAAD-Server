using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using milktea_server.Dtos.Order;
using milktea_server.Dtos.Response;
using milktea_server.Dtos.Statistic;
using milktea_server.Dtos.User;
using milktea_server.Enums;
using milktea_server.Interfaces.Repositories;
using milktea_server.Interfaces.Services;
using milktea_server.Models;
using milktea_server.Queries;
using milktea_server.Utilities;

namespace milktea_server.Services
{
    public class UserService : IUserService
    {
        private readonly IConfiguration _configuration;
        private readonly ICartRepository _cartRepo;
        private readonly IOrderRepository _orderRepo;
        private readonly IAccountRepository _accountRepo;
        private readonly ICustomerRepository _customerRepo;
        private readonly IAdminRepository _adminRepo;
        private readonly IMailerService _mailerService;

        public UserService(
            IConfiguration configuration,
            ICartRepository cartRepo,
            IOrderRepository orderRepo,
            IAccountRepository accountRepo,
            ICustomerRepository customerRepo,
            IAdminRepository adminRepo,
            IMailerService mailerService
        )
        {
            _configuration = configuration;
            _cartRepo = cartRepo;
            _orderRepo = orderRepo;
            _accountRepo = accountRepo;
            _customerRepo = customerRepo;
            _adminRepo = adminRepo;
            _mailerService = mailerService;
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
                        IsActive = cus.Account != null && cus.Account.IsActive,
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

        public async Task<ServiceResponse> UpdateCustomerProfile(UpdateCustomerDto updateCustomerDto, int customerId)
        {
            var targetCustomer = await _customerRepo.GetCustomerById(customerId);
            if (targetCustomer == null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.NOT_FOUND,
                    Success = false,
                    Message = ErrorMessage.USER_NOT_FOUND,
                };
            }

            targetCustomer.FirstName = updateCustomerDto.FirstName;
            targetCustomer.LastName = updateCustomerDto.LastName;

            if (!string.IsNullOrEmpty(updateCustomerDto.Avatar))
            {
                targetCustomer.Avatar = updateCustomerDto.Avatar;
            }

            targetCustomer.Address = string.IsNullOrEmpty(updateCustomerDto.Address) ? null : updateCustomerDto.Address;
            targetCustomer.PhoneNumber = string.IsNullOrEmpty(updateCustomerDto.PhoneNumber) ? null : updateCustomerDto.PhoneNumber;

            if (!string.IsNullOrEmpty(updateCustomerDto.Email))
            {
                var customerWithThisEmail = await _customerRepo.GetCustomerByEmail(updateCustomerDto.Email);
                if (customerWithThisEmail != null && customerWithThisEmail.Id != customerId)
                {
                    return new ServiceResponse
                    {
                        Status = ResStatusCode.CONFLICT,
                        Success = false,
                        Message = ErrorMessage.EMAIL_EXISTED,
                    };
                }

                targetCustomer.Email = updateCustomerDto.Email;
            }
            else
            {
                targetCustomer.Email = null;
            }

            await _customerRepo.UpdateCustomer(targetCustomer);

            return new ServiceResponse
            {
                Status = ResStatusCode.OK,
                Success = true,
                Message = SuccessMessage.UPDATE_USER_SUCCESSFULLY,
            };
        }

        public async Task<ServiceResponse<List<Admin>>> GetAllAdmins(BaseQueryObject queryObject)
        {
            var (admins, total) = await _adminRepo.GetAllAdmins(queryObject);

            return new ServiceResponse<List<Admin>>
            {
                Status = ResStatusCode.OK,
                Success = true,
                Data = admins,
                Total = total,
                Took = admins.Count,
            };
        }

        public async Task<ServiceResponse> CreateNewAdmin(CreateAdminDto createAdminDto, int authUserId, string locale)
        {
            var adminWithSameEmail = await _adminRepo.GetAdminByEmailIncludeInactive(createAdminDto.Email);
            if (adminWithSameEmail != null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.CONFLICT,
                    Success = false,
                    Message = ErrorMessage.EMAIL_EXISTED,
                };
            }

            var adminWithSamePhone = await _adminRepo.GetAdminByPhoneNumberIncludeInactive(createAdminDto.PhoneNumber);
            if (adminWithSamePhone != null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.CONFLICT,
                    Success = false,
                    Message = ErrorMessage.PHONE_NUMBER_EXISTED,
                };
            }

            string randomUsername = RandomStringGenerator.GenerateRandomString(16);
            string randomPassword = RandomStringGenerator.GenerateRandomString(16);

            var newAccount = new Account
            {
                Username = randomUsername,
                Password = BCrypt.Net.BCrypt.HashPassword(randomPassword),
                Role = UserRole.Admin,
            };
            await _accountRepo.AddAccount(newAccount);

            var newAdmin = new Admin
            {
                FirstName = createAdminDto.FirstName,
                LastName = createAdminDto.LastName,
                Email = createAdminDto.Email,
                PhoneNumber = createAdminDto.PhoneNumber,
                Avatar = string.IsNullOrWhiteSpace(createAdminDto.Avatar)
                    ? _configuration["Application:DefaultUserAvatar"]
                    : createAdminDto.Avatar,
                AccountId = newAccount.Id,
                CreatedById = authUserId,
            };
            await _adminRepo.AddAdmin(newAdmin);

            await _mailerService.SendWelcomeNewAdminEmail(
                createAdminDto.Email,
                $"{createAdminDto.LastName} {createAdminDto.FirstName}",
                randomUsername,
                randomPassword,
                $"{_configuration["Application:ClientUrl"]}/profile/change-password",
                locale
            );

            return new ServiceResponse
            {
                Status = ResStatusCode.CREATED,
                Success = true,
                Message = SuccessMessage.CREATE_ADMIN_SUCCESSFULLY,
            };
        }

        public async Task<ServiceResponse> UpdateAdminProfile(UpdateAdminDto updateAdminDto, int adminId)
        {
            var targetAdmin = await _adminRepo.GetAdminById(adminId);
            if (targetAdmin == null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.NOT_FOUND,
                    Success = false,
                    Message = ErrorMessage.USER_NOT_FOUND,
                };
            }

            targetAdmin.FirstName = updateAdminDto.FirstName;
            targetAdmin.LastName = updateAdminDto.LastName;

            if (!string.IsNullOrEmpty(updateAdminDto.Avatar))
            {
                targetAdmin.Avatar = updateAdminDto.Avatar;
            }

            var adminWithThisPhoneNumber = await _adminRepo.GetAdminByPhoneNumber(updateAdminDto.PhoneNumber);
            if (adminWithThisPhoneNumber != null && adminWithThisPhoneNumber.Id != adminId)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.CONFLICT,
                    Success = false,
                    Message = ErrorMessage.PHONE_NUMBER_EXISTED,
                };
            }

            targetAdmin.PhoneNumber = updateAdminDto.PhoneNumber;

            await _adminRepo.UpdateAdmin(targetAdmin);

            return new ServiceResponse
            {
                Status = ResStatusCode.OK,
                Success = true,
                Message = SuccessMessage.UPDATE_USER_SUCCESSFULLY,
            };
        }

        public async Task<ServiceResponse> ToggleAdminActiveStatus(int adminId, int authUserId)
        {
            if (adminId == authUserId)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.FORBIDDEN,
                    Success = false,
                    Message = ErrorMessage.NO_PERMISSION,
                };
            }

            var targetAdmin = await _adminRepo.GetAdminByIdIncludeInactive(adminId);
            if (targetAdmin == null)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.NOT_FOUND,
                    Success = false,
                    Message = ErrorMessage.USER_NOT_FOUND,
                };
            }

            bool newActiveStatus = !targetAdmin.Account!.IsActive;
            targetAdmin.Account!.IsActive = newActiveStatus;

            await _adminRepo.UpdateAdmin(targetAdmin);

            if (newActiveStatus == false)
            {
                await _orderRepo.RejectAllOrdersProcessedByStaff(adminId);
            }

            return new ServiceResponse
            {
                Status = ResStatusCode.OK,
                Success = true,
                Message = newActiveStatus ? SuccessMessage.REACTIVATE_ACCOUNT_SUCCESSFULLY : SuccessMessage.DEACTIVATE_ACCOUNT_SUCCESSFULLY,
            };
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
