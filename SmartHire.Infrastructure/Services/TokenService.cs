using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SmartHire.Core.Entities;
using SmartHire.Core.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SmartHire.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _key;
        private readonly IConfiguration _config;

        public TokenService(IConfiguration config)
        {
            _config = config;
            var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET") 
                            ?? config["JwtSettings:SecretKey"];
            
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new InvalidOperationException("JWT Secret Key is not configured.");
            }

            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        }

        public string CreateToken(AppUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.NameId, user.Id),
                new Claim(JwtRegisteredClaimNames.GivenName, user.FullName)
            };

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(_config["JwtSettings:ExpiryMinutes"] ?? "1440")),
                SigningCredentials = creds,
                Issuer = _config["JwtSettings:Issuer"],
                Audience = _config["JwtSettings:Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
