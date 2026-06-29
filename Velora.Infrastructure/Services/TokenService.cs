using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Velora.Core.DTOs;
using Velora.Core.Entities;
using Velora.Core.Interfaces;
using Velora.Infrastructure.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Velora.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _key;
        private readonly IConfiguration _config;
        private readonly UserManager<AppUser> _userManager;
        private readonly AppDbContext _db;

        public TokenService(IConfiguration config, UserManager<AppUser> userManager, AppDbContext db)
        {
            _config = config;
            _userManager = userManager;
            _db = db;
            var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET") 
                            ?? config["JwtSettings:SecretKey"];
            
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new InvalidOperationException("JWT Secret Key is not configured.");
            }

            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        }

        public async Task<string> CreateToken(AppUser user, CancellationToken cancellationToken = default)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.NameId, user.Id),
                new Claim(JwtRegisteredClaimNames.GivenName, user.FullName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

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

        public async Task<UserDto> GenerateTokens(AppUser user, string ipAddress, CancellationToken cancellationToken = default)
        {

            return new UserDto();
        }

        public async Task<UserDto> RefreshToken(string token, string refreshToken, string ipAddress, CancellationToken cancellationToken = default)
        {
            return new UserDto();
        }

        public async Task<bool> RevokeToken(string refreshToken, string ipAddress, CancellationToken cancellationToken = default)
        {
            
            return true;
        }
    }
}

