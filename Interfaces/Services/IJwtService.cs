using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using milktea_server.Enums;
using milktea_server.Models;

namespace milktea_server.Interfaces.Services
{
    public interface IJwtService
    {
        string GenerateAccessToken(AppUser user, UserRole role);
        string GenerateRefreshToken(Account account);
        string GenerateResetPasswordToken(Customer customer);
        bool VerifyRefreshToken(string refreshToken, out ClaimsPrincipal? principal);
        bool VerifyResetPasswordToken(string resetPasswordToken, out ClaimsPrincipal? principal);
    }
}
