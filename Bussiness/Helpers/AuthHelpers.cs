using Bussiness.Interfaces;
using Data;
using Domain.DTOs.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Bussiness.Helpers
{
    public class AuthHelpers
    {
        private readonly IConfiguration _configuration = null!;

        public AuthHelpers(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }
        public string GenerateAccessToken(DTO_UserAuthInfo user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.MobilePhone, user.PhoneNumber ?? string.Empty)
            };

            if (user.Role != null)
            {
                foreach (var roleName in user.Role)
                {
                    if (!string.IsNullOrWhiteSpace(roleName))
                        claims.Add(new Claim(ClaimTypes.Role, roleName));
                }
            }

            var secretKey = _configuration["GamersPlat_JKey"];

            if (string.IsNullOrWhiteSpace(secretKey))
                throw new Exception("JWT secret key is not configured.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "GamersPlat",
                audience: "GamersPlatUsers",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var bytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }
    }
}
