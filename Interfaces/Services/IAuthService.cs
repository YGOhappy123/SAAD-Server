using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using milktea_server.Dtos.Auth;
using milktea_server.Dtos.Response;
using milktea_server.Models;

namespace milktea_server.Interfaces.Services
{
    public interface IAuthService
    {
        Task<ServiceResponse<AppUser>> SignIn(SignInDto signInDto);
        Task<ServiceResponse<Customer>> SignUpCustomerAccount(SignUpDto signUpDto);
        Task<ServiceResponse> RefreshToken(RefreshTokenDto refreshTokenDto);
        Task<ServiceResponse> ForgotPassword(ForgotPasswordDto forgotPasswordDto, string locale);
        Task<ServiceResponse> ResetPassword(string resetPasswordToken, ResetPasswordDto resetPasswordDto);
        Task<ServiceResponse<Customer>> GoogleAuthentication(GoogleAuthDto googleAuthDto, string locale);
        Task<ServiceResponse> DeactivateAccount(DeactivateAccountDto deactivateAccountDto, int authUserId, string authUserRole);
    }
}
