using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using milktea_server.Dtos.Auth;
using milktea_server.Dtos.Response;
using milktea_server.Enums;
using milktea_server.Interfaces.Repositories;
using milktea_server.Interfaces.Services;
using milktea_server.Models;
using milktea_server.Utilities;

namespace milktea_server.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IAccountRepository _accountRepo;
        private readonly ICustomerRepository _customerRepo;
        private readonly IAdminRepository _adminRepo;
        private readonly IJwtService _jwtService;
        private readonly IMailerService _mailerService;

        public AuthService(
            IConfiguration configuration,
            IAccountRepository accountRepo,
            ICustomerRepository customerRepo,
            IAdminRepository adminRepo,
            IJwtService jwtService,
            IMailerService mailerService
        )
        {
            _configuration = configuration;
            _accountRepo = accountRepo;
            _customerRepo = customerRepo;
            _adminRepo = adminRepo;
            _jwtService = jwtService;
            _mailerService = mailerService;
        }

        public async Task<ServiceResponse<AppUser>> SignIn(SignInDto signInDto)
        {
            var existedAccount = await _accountRepo.GetAccountByUsername(signInDto.Username);

            if (
                existedAccount == null
                || !existedAccount.IsActive
                || !BCrypt.Net.BCrypt.Verify(signInDto.Password, existedAccount.Password)
            )
            {
                return new ServiceResponse<AppUser>
                {
                    Status = ResStatusCode.UNAUTHORIZED,
                    Success = false,
                    Message = ErrorMessage.INVALID_CREDENTIALS,
                };
            }
            else
            {
                AppUser? userData =
                    (existedAccount.Role == UserRole.Customer)
                        ? await _customerRepo.GetCustomerByAccountId(existedAccount.Id)
                        : await _adminRepo.GetAdminByAccountId(existedAccount.Id);

                return new ServiceResponse<AppUser>
                {
                    Status = ResStatusCode.OK,
                    Success = true,
                    Message = SuccessMessage.SIGN_IN_SUCCESSFULLY,
                    Data = userData,
                    AccessToken = _jwtService.GenerateAccessToken(userData!, existedAccount.Role),
                    RefreshToken = _jwtService.GenerateRefreshToken(existedAccount),
                };
            }
        }

        public async Task<ServiceResponse<Customer>> SignUpCustomerAccount(SignUpDto signUpDto)
        {
            var existedAccount = await _accountRepo.GetAccountByUsername(signUpDto.Username);
            if (existedAccount != null)
            {
                return new ServiceResponse<Customer>
                {
                    Status = ResStatusCode.CONFLICT,
                    Success = false,
                    Message = ErrorMessage.USERNAME_EXISTED,
                };
            }

            var newAccount = new Account { Username = signUpDto.Username, Password = BCrypt.Net.BCrypt.HashPassword(signUpDto.Password) };
            await _accountRepo.AddAccount(newAccount);

            var newCustomer = new Customer
            {
                FirstName = signUpDto.FirstName,
                LastName = signUpDto.LastName,
                AccountId = newAccount.Id,
                Avatar = _configuration["Application:DefaultUserAvatar"],
            };
            await _customerRepo.AddCustomer(newCustomer);

            return new ServiceResponse<Customer>
            {
                Status = ResStatusCode.CREATED,
                Success = true,
                Message = SuccessMessage.SIGN_UP_SUCCESSFULLY,
                Data = newCustomer,
                AccessToken = _jwtService.GenerateAccessToken(newCustomer!, UserRole.Customer),
                RefreshToken = _jwtService.GenerateRefreshToken(newAccount),
            };
        }

        public async Task<ServiceResponse> RefreshToken(RefreshTokenDto refreshTokenDto)
        {
            if (_jwtService.VerifyRefreshToken(refreshTokenDto.RefreshToken, out var principal))
            {
                var accountId = principal!.FindFirst(ClaimTypes.Name)!.Value;
                var account = await _accountRepo.GetAccountById(int.Parse(accountId));

                if (account == null || !account.IsActive)
                {
                    return new ServiceResponse
                    {
                        Status = ResStatusCode.UNAUTHORIZED,
                        Success = false,
                        Message = ErrorMessage.INVALID_CREDENTIALS,
                    };
                }

                AppUser? userData =
                    (account.Role == UserRole.Customer)
                        ? await _customerRepo.GetCustomerByAccountId(account.Id)
                        : await _adminRepo.GetAdminByAccountId(account.Id);

                return new ServiceResponse
                {
                    Status = ResStatusCode.OK,
                    Success = true,
                    Message = SuccessMessage.REFRESH_TOKEN_SUCCESSFULLY,
                    AccessToken = _jwtService.GenerateAccessToken(userData!, account.Role),
                };
            }
            else
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.UNAUTHORIZED,
                    Success = false,
                    Message = ErrorMessage.INVALID_CREDENTIALS,
                };
            }
        }

        public async Task<ServiceResponse> ForgotPassword(ForgotPasswordDto forgotPasswordDto, string locale)
        {
            var existedCustomer = await _customerRepo.GetCustomerByEmail(forgotPasswordDto.Email, isAccountIncluded: true);

            if (existedCustomer == null || existedCustomer.Account == null || !existedCustomer.Account.IsActive)
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.NOT_FOUND,
                    Success = false,
                    Message = ErrorMessage.USER_NOT_FOUND,
                };
            }
            else
            {
                await _mailerService.SendResetPasswordEmail(
                    forgotPasswordDto.Email,
                    $"{existedCustomer.LastName} {existedCustomer.FirstName}",
                    $"{_configuration["Application:ClientUrl"]}?token={_jwtService.GenerateResetPasswordToken(existedCustomer)}",
                    locale
                );

                return new ServiceResponse
                {
                    Status = ResStatusCode.OK,
                    Success = true,
                    Message = SuccessMessage.RESET_PASSWORD_EMAIL_SENT,
                };
            }
        }

        public async Task<ServiceResponse> ResetPassword(string resetPasswordToken, ResetPasswordDto resetPasswordDto)
        {
            if (_jwtService.VerifyResetPasswordToken(resetPasswordToken, out var principal))
            {
                var email = principal!.FindFirst(ClaimTypes.Email)!.Value;
                var account = await _accountRepo.GetCustomerAccountByEmail(email);

                if (account == null || !account.IsActive)
                {
                    return new ServiceResponse
                    {
                        Status = ResStatusCode.UNAUTHORIZED,
                        Success = false,
                        Message = ErrorMessage.INVALID_CREDENTIALS,
                    };
                }

                account.Password = BCrypt.Net.BCrypt.HashPassword(resetPasswordDto.Password);
                await _accountRepo.UpdateAccount(account);

                return new ServiceResponse
                {
                    Status = ResStatusCode.OK,
                    Success = true,
                    Message = SuccessMessage.RESET_PASSWORD_SUCCESSFULLY,
                };
            }
            else
            {
                return new ServiceResponse
                {
                    Status = ResStatusCode.UNAUTHORIZED,
                    Success = false,
                    Message = ErrorMessage.INVALID_CREDENTIALS,
                };
            }
        }

        public async Task<ServiceResponse<Customer>> GoogleAuthentication(GoogleAuthDto googleAuthDto, string locale)
        {
            var googleUserInfo = await FetchGoogleUserInfoAsync(googleAuthDto.GoogleAccessToken);
            if (googleUserInfo == null || !googleUserInfo.EmailVerified)
            {
                return new ServiceResponse<Customer>
                {
                    Status = ResStatusCode.UNAUTHORIZED,
                    Success = false,
                    Message = ErrorMessage.GOOGLE_AUTH_FAILED,
                };
            }

            var existedAccount = await _accountRepo.GetCustomerAccountByEmail(googleUserInfo.Email);
            if (existedAccount == null)
            {
                string randomUsername = RandomStringGenerator.GenerateRandomString(16);
                string randomPassword = RandomStringGenerator.GenerateRandomString(16);

                var newAccount = new Account { Username = randomUsername, Password = BCrypt.Net.BCrypt.HashPassword(randomPassword) };
                await _accountRepo.AddAccount(newAccount);

                var newCustomer = new Customer
                {
                    FirstName = googleUserInfo.FirstName,
                    LastName = googleUserInfo.LastName,
                    AccountId = newAccount.Id,
                    Avatar = googleUserInfo.Picture ?? _configuration["Application:DefaultUserAvatar"],
                    Email = googleUserInfo.Email,
                };
                await _customerRepo.AddCustomer(newCustomer);

                await _mailerService.SendGoogleRegistrationSuccessEmail(
                    googleUserInfo.Email,
                    $"{newCustomer.LastName} {newCustomer.FirstName}",
                    randomUsername,
                    randomPassword,
                    $"{_configuration["Application:ClientUrl"]}/profile/change-password",
                    locale
                );

                return new ServiceResponse<Customer>
                {
                    Status = ResStatusCode.CREATED,
                    Success = true,
                    Message = SuccessMessage.GOOGLE_AUTH_SUCCESSFULLY,
                    Data = newCustomer,
                    AccessToken = _jwtService.GenerateAccessToken(newCustomer!, UserRole.Customer),
                    RefreshToken = _jwtService.GenerateRefreshToken(newAccount),
                };
            }
            else
            {
                var customerData = await _customerRepo.GetCustomerByAccountId(existedAccount.Id);

                return new ServiceResponse<Customer>
                {
                    Status = ResStatusCode.OK,
                    Success = true,
                    Message = SuccessMessage.GOOGLE_AUTH_SUCCESSFULLY,
                    Data = customerData,
                    AccessToken = _jwtService.GenerateAccessToken(customerData!, UserRole.Customer),
                    RefreshToken = _jwtService.GenerateRefreshToken(existedAccount),
                };
            }
        }

        private async Task<GoogleUserInfoDto?> FetchGoogleUserInfoAsync(string googleAccessToken)
        {
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", googleAccessToken);

            var response = await httpClient.GetAsync(_configuration["GoogleApi:OAuthEndPoint"]);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<GoogleUserInfoDto>(json);
        }
    }
}
