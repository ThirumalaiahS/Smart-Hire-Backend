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
            var jti = Guid.NewGuid().ToString();
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.NameId, user.Id),
                new Claim(JwtRegisteredClaimNames.GivenName, user.FullName),
                new Claim(JwtRegisteredClaimNames.Jti, jti)
            };

            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            var exporyMinutes = double.Parse(_config["JwtSettings:ExpiryMinutes"] ?? "1440");
            var tokenExpiry = DateTime.UtcNow.AddMinutes(exporyMinutes);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = tokenExpiry,
                SigningCredentials = creds,
                Issuer = _config["JwtSettings:Issuer"],
                Audience = _config["JwtSettings:Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(securityToken);

            // Generate refresh token
            var refreshTokenString = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(64));
            var refreshTokenExpiryDays = double.Parse(_config["JwtSettings:RefreshTokenExpiryDays"] ?? "7");
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(refreshTokenExpiryDays);

            // Get SystemUser
            var systemUser = await _db.SystemUsers.FirstOrDefaultAsync(x => x.IdentityUserId == user.Id, cancellationToken);
            if (systemUser == null)
            {
                throw new InvalidOperationException("System user not found associated with Identity user.");
            }

            var refreshToken = new RefreshToken
            {
                SysUserId = systemUser.UserId,
                Token = refreshTokenString,
                JwtId = jti,
                ExpiresOn = refreshTokenExpiry,
                CreatedOn = DateTime.UtcNow,
                CreatedByIp = ipAddress,
                IsRevoked = false,
                IsUsed = false,
            };

            await _db.RefreshTokens.AddAsync(refreshToken, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);

            return new UserDto
            {
                Email = user.Email!,
                FullName = user.FullName,
                Token = tokenString,
                TokenExpiry = tokenExpiry,
                RefreshToken = refreshTokenString,
                RefreshTokenExpiry = refreshTokenExpiry
            };
        }

        public async Task<UserDto> RefreshToken(string token, string refreshToken, string ipAddress, CancellationToken cancellationToken = default)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken validatedToken;

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _key,
                ValidateIssuer = true,
                ValidIssuer = _config["JwtSettings:Issuer"],
                ValidateAudience = true,
                ValidAudience = _config["JwtSettings:Audience"],
                ValidateLifetime = false, // We check expired tokens!
                ClockSkew = TimeSpan.Zero
            };

            ClaimsPrincipal principal;
            try
            {
                principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out validatedToken);
            }
            catch (Exception)
            {
                throw new SecurityTokenException("Invalid access token");
            }

            var jwtSecurityToken = validatedToken as JwtSecurityToken;
            if (jwtSecurityToken == null && !jwtSecurityToken!.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid access token algorithm");
            }

            var userId = principal.FindFirst(JwtRegisteredClaimNames.NameId)?.Value;
            var jti = principal.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(jti))
            {
                throw new SecurityTokenException("Invalid access token claims");
            }

            var storedRefreshToken = await _db.RefreshTokens
                .Include(r => r.SystemUser)
                .FirstOrDefaultAsync(r => r.Token == refreshToken, cancellationToken);

            if (storedRefreshToken == null)
            {
                throw new SecurityTokenException("Refresh token does not exist");
            }

            // Check if refresh token has already been used
            if (storedRefreshToken.IsUsed)
            {
                // Replay attack: Revoke all tokens for this user
                var userTokens = await _db.RefreshTokens
                    .Where(r => r.SysUserId == storedRefreshToken.SysUserId)
                    .ToListAsync(cancellationToken);

                foreach (var t in userTokens)
                {
                    if (!t.IsRevoked)
                    {
                        t.IsRevoked = true;
                        t.RevokedOn = DateTime.UtcNow;
                        t.RevokedByIp = ipAddress;
                    }
                }
                await _db.SaveChangesAsync(cancellationToken);
                throw new SecurityTokenException("Refresh token has been used. Potential fraud detected. All tokens revoked.");
            }

            if (storedRefreshToken.IsRevoked)
            {
                throw new SecurityTokenException("Refresh token has been revoked");
            }

            if (storedRefreshToken.ExpiresOn < DateTime.UtcNow)
            {
                throw new SecurityTokenException("Refresh token has expired");
            }

            if (storedRefreshToken.JwtId != jti)
            {
                throw new SecurityTokenException("Refresh token does not match access token");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || !user.IsActive)
            {
                throw new SecurityTokenException("User not found or deactivated");
            }

            // Rotate Refresh Token
            var newRefreshTokenString = Convert.ToBase64String(System.Security.Cryptography.RandomNumberGenerator.GetBytes(64));
            var refreshTokenExpiryDays = double.Parse(_config["JwtSettings:RefreshTokenExpiryDays"] ?? "7");
            var newRefreshTokenExpiry = DateTime.UtcNow.AddDays(refreshTokenExpiryDays);

            // Mark old token as used and replaced
            storedRefreshToken.IsUsed = true;
            storedRefreshToken.RevokedOn = DateTime.UtcNow;
            storedRefreshToken.RevokedByIp = ipAddress;
            storedRefreshToken.ReplacedByToken = newRefreshTokenString;

            // Generate new access token
            var newJwtId = Guid.NewGuid().ToString();
            var newClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.NameId, user.Id),
                new Claim(JwtRegisteredClaimNames.GivenName, user.FullName),
                new Claim(JwtRegisteredClaimNames.Jti, newJwtId)
            };

            var roles = await _userManager.GetRolesAsync(user);
            newClaims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
            var expiryMinutes = double.Parse(_config["JwtSettings:ExpiryMinutes"] ?? "1440");
            var newTokenExpiry = DateTime.UtcNow.AddMinutes(expiryMinutes);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(newClaims),
                Expires = newTokenExpiry,
                SigningCredentials = creds,
                Issuer = _config["JwtSettings:Issuer"],
                Audience = _config["JwtSettings:Audience"]
            };

            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var newTokenString = tokenHandler.WriteToken(securityToken);

            var newRefreshToken = new RefreshToken
            {
                SysUserId = storedRefreshToken.SysUserId,
                Token = newRefreshTokenString,
                JwtId = newJwtId,
                ExpiresOn = newRefreshTokenExpiry,
                CreatedOn = DateTime.UtcNow,
                CreatedByIp = ipAddress,
                IsRevoked = false,
                IsUsed = false
            };

            await _db.RefreshTokens.AddAsync(newRefreshToken, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);

            return new UserDto
            {
                Email = user.Email!,
                FullName = user.FullName,
                Token = newTokenString,
                TokenExpiry = newTokenExpiry,
                RefreshToken = newRefreshTokenString,
                RefreshTokenExpiry = newRefreshTokenExpiry
            };
        }

        public async Task<bool> RevokeToken(string refreshToken, string ipAddress, CancellationToken cancellationToken = default)
        {
            var storedToken = await _db.RefreshTokens.FirstOrDefaultAsync(x => x.Token == refreshToken, cancellationToken);

            if (storedToken == null || storedToken.IsRevoked || storedToken.IsUsed)
            {
                return false;
            }

            storedToken.IsRevoked = true;
            storedToken.RevokedOn = DateTime.UtcNow;
            storedToken.RevokedByIp = ipAddress;

            await _db.SaveChangesAsync(cancellationToken);            
            return true;
        }
    }
}

