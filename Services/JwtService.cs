using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using milktea_server.Enums;
using milktea_server.Interfaces.Services;
using milktea_server.Models;

namespace milktea_server.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;
        private readonly JwtSecurityTokenHandler _tokenHandler;

        public JwtService(IConfiguration configuration, JwtSecurityTokenHandler tokenHandler)
        {
            _configuration = configuration;
            _tokenHandler = tokenHandler;
        }

        private string GenerateToken(Claim[] claims, string tokenKeyPath, int expirationInMins = 60)
        {
            var key = Encoding.ASCII.GetBytes(_configuration[tokenKeyPath]!);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddMinutes(expirationInMins),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            };

            var token = _tokenHandler.CreateToken(tokenDescriptor);
            return _tokenHandler.WriteToken(token);
        }

        public string GenerateAccessToken(AppUser user, UserRole role)
        {
            var claims = new[] { new Claim(ClaimTypes.Name, user.Id.ToString()), new Claim(ClaimTypes.Role, role.ToString()) };

            return GenerateToken(claims, "Jwt:AccessTokenSecret", 30);
        }

        public string GenerateRefreshToken(Account account)
        {
            var claims = new[] { new Claim(ClaimTypes.Name, account.Id.ToString()) };

            return GenerateToken(claims, "Jwt:RefreshTokenSecret", 60 * 24 * 7);
        }

        public string GenerateResetPasswordToken(Customer customer)
        {
            var claims = new[] { new Claim(ClaimTypes.Email, customer.Email!.ToString()) };

            return GenerateToken(claims, "Jwt:ResetPasswordTokenSecret", 10);
        }

        private ClaimsPrincipal? VerifyToken(string token, string tokenKeyPath)
        {
            var key = Encoding.ASCII.GetBytes(_configuration[tokenKeyPath]!);

            try
            {
                var principal = _tokenHandler.ValidateToken(
                    token,
                    new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ClockSkew = TimeSpan.Zero,
                    },
                    out SecurityToken validatedToken
                );

                return principal;
            }
            catch
            {
                return null;
            }
        }

        public bool VerifyRefreshToken(string refreshToken, out ClaimsPrincipal? principal)
        {
            principal = VerifyToken(refreshToken, "Jwt:RefreshTokenSecret");
            return principal != null;
        }

        public bool VerifyResetPasswordToken(string resetPasswordToken, out ClaimsPrincipal? principal)
        {
            principal = VerifyToken(resetPasswordToken, "Jwt:ResetPasswordTokenSecret");
            return principal != null;
        }
    }
}
