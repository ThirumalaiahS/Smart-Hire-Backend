using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SmartHire.API.Controllers;
using SmartHire.Core.Common;
using SmartHire.Core.DTOs;
using SmartHire.Core.Entities;
using SmartHire.Core.Interfaces;
using System.Security.Claims;

namespace SmartHire.Tests.Auth
{
    public class AuthControllerTests
    {
        private readonly Mock<UserManager<AppUser>> _mockUserManager;
        private readonly Mock<SignInManager<AppUser>> _mockSignInManager;
        private readonly Mock<RoleManager<IdentityRole>> _mockRoleManager;
        private readonly Mock<ITokenService> _mockTokenService;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mockUserManager = MockUserManager<AppUser>();
            _mockRoleManager = MockRoleManager<IdentityRole>();
            
            var contextAccessor = new Mock<IHttpContextAccessor>();
            var userPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<AppUser>>();
            
            _mockSignInManager = new Mock<SignInManager<AppUser>>(
                _mockUserManager.Object,
                contextAccessor.Object,
                userPrincipalFactory.Object,
                null!, null!, null!, null!);
             
            _mockTokenService = new Mock<ITokenService>();

            _controller = new AuthController(
                _mockUserManager.Object,
                _mockSignInManager.Object,
                _mockRoleManager.Object,
                _mockTokenService.Object);
        }

        private static Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            return new Mock<UserManager<TUser>>(store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
        }

        private static Mock<RoleManager<TRole>> MockRoleManager<TRole>() where TRole : class
        {
            var store = new Mock<IRoleStore<TRole>>();
            return new Mock<RoleManager<TRole>>(store.Object, null!, null!, null!, null!);
        }

        [Fact]
        public async Task Register_ReturnsUserDto_WhenSuccessful()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Email = "test@example.com",
                Password = "Password123!",
                FullName = "Test User",
                MobileNumber = "1234567890",
                Role = CommonEnums.UserRole.Candidate
            };

            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            _mockTokenService.Setup(x => x.CreateToken(It.IsAny<AppUser>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync("fake-token");

            // Act
            var result = await _controller.Register(registerDto, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<UserDto>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal(registerDto.Email, response.Data!.Email);
            Assert.Equal("fake-token", response.Data.Token);
        }

        [Fact]
        public async Task Login_ReturnsUnauthorized_WhenUserIsDeactivated()
        {
            // Arrange
            var loginDto = new LoginDto { Email = "test@example.com", Password = "Password123!" };
            var user = new AppUser { Email = "test@example.com", IsActive = false };

            _mockUserManager.Setup(x => x.FindByEmailAsync(loginDto.Email))
                .ReturnsAsync(user);

            // Act
            var result = await _controller.Login(loginDto, CancellationToken.None);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            var response = Assert.IsType<ApiResponse<UserDto>>(unauthorizedResult.Value);
            Assert.False(response.Success);
            Assert.Contains("Account is deactivated", response.Errors[0]);
        }

        [Fact]
        public async Task DeactivateAccount_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var user = new AppUser { Email = "test@example.com", IsActive = true };
            _mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);
            _mockUserManager.Setup(x => x.UpdateAsync(It.IsAny<AppUser>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.DeactivateAccount(CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal("Account deactivated successfully.", response.Message);
            Assert.False(user.IsActive);
        }

        [Fact]
        public async Task DeleteUser_ReturnsOk_WhenAdminDeletesUser()
        {
            // Arrange
            var userId = "user-id";
            var user = new AppUser { Id = userId };
            _mockUserManager.Setup(x => x.FindByIdAsync(userId))
                .ReturnsAsync(user);
            _mockUserManager.Setup(x => x.DeleteAsync(user))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.DeleteUser(userId, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal("User deleted successfully.", response.Message);
        }

        [Fact]
        public async Task ForgotPassword_ReturnsToken_WhenUserExists()
        {
            // Arrange
            var forgotPasswordDto = new ForgotPasswordDto { Email = "test@example.com" };
            var user = new AppUser { Email = "test@example.com" };
            _mockUserManager.Setup(x => x.FindByEmailAsync(forgotPasswordDto.Email))
                .ReturnsAsync(user);
            _mockUserManager.Setup(x => x.GeneratePasswordResetTokenAsync(user))
                .ReturnsAsync("reset-token");

            // Act
            var result = await _controller.ForgotPassword(forgotPasswordDto, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(okResult.Value);
            Assert.True(response.Success);
            // Using reflection or dynamic to check the anonymous object inside response.Data
            var data = response.Data as dynamic;
            Assert.NotNull(data);
        }

        [Fact]
        public async Task ResetPassword_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var resetPasswordDto = new ResetPasswordDto 
            { 
                Email = "test@example.com", 
                Token = "reset-token", 
                NewPassword = "NewPassword123!" 
            };
            var user = new AppUser { Email = "test@example.com" };
            _mockUserManager.Setup(x => x.FindByEmailAsync(resetPasswordDto.Email))
                .ReturnsAsync(user);
            _mockUserManager.Setup(x => x.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.ResetPassword(resetPasswordDto, CancellationToken.None);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ApiResponse<object>>(okResult.Value);
            Assert.True(response.Success);
            Assert.Equal("Password reset successfully.", response.Message);
        }
    }
}
