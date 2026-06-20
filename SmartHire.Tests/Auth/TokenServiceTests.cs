using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using SmartHire.Core.Entities;
using SmartHire.Infrastructure.Services;
using System.IdentityModel.Tokens.Jwt;
using Xunit;

namespace SmartHire.Tests.Auth
{
    public class TokenServiceTests
    {
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly Mock<UserManager<AppUser>> _mockUserManager;
        private readonly TokenService _tokenService;

        public TokenServiceTests()
        {
            _mockConfig = new Mock<IConfiguration>();
            _mockUserManager = MockUserManager<AppUser>();

            // Setup config
            _mockConfig.Setup(x => x["JwtSettings:SecretKey"]).Returns("v9y$B&E)H@McQfTjWnZr4u7x!A%C*F-JaNdRgUkXp2s5v8y/B?E(G+KbPeShVmYq");
            _mockConfig.Setup(x => x["JwtSettings:Issuer"]).Returns("SmartHireAPI");
            _mockConfig.Setup(x => x["JwtSettings:Audience"]).Returns("SmartHireClient");
            _mockConfig.Setup(x => x["JwtSettings:ExpiryMinutes"]).Returns("1440");

            _tokenService = new TokenService(_mockConfig.Object, _mockUserManager.Object);
        }

        private static Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            return new Mock<UserManager<TUser>>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        }

        [Fact]
        public async Task CreateToken_ReturnsTokenWithRoles()
        {
            // Arrange
            var user = new AppUser
            {
                Id = "user-id",
                Email = "test@example.com",
                FullName = "Test User"
            };
            var roles = new List<string> { "Admin", "Recruiter" };

            _mockUserManager.Setup(x => x.GetRolesAsync(user))
                .ReturnsAsync(roles);

            // Act
            var token = await _tokenService.CreateToken(user, CancellationToken.None);

            // Assert
            Assert.NotNull(token);
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            Assert.Equal("test@example.com", jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Email).Value);
            var roleClaims = jwtToken.Claims.Where(c => c.Type == "role").Select(c => c.Value).ToList();
            Assert.Contains("Admin", roleClaims);
            Assert.Contains("Recruiter", roleClaims);
        }
    }
}
