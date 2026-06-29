using Velora.Core.DTOs;
using Xunit;
using static Velora.Core.Common.CommonEnums;

namespace Velora.Tests.Auth
{
    public class ValidatorTests
    {
        private readonly RegisterDtoValidator _registerValidator;
        private readonly LoginDtoValidator _loginValidator;

        public ValidatorTests()
        {
            _registerValidator = new RegisterDtoValidator();
            _loginValidator = new LoginDtoValidator();
        }

        [Theory]
        [InlineData("test@example.com", "Password123!", "Full Name", "+1234567890", UserRole.Candidate, true)]
        [InlineData("invalid-email", "Password123!", "Full Name", "+1234567890", UserRole.Candidate, false)]
        [InlineData("test@example.com", "short", "Full Name", "+1234567890", UserRole.Candidate, false)] // Password < 8
        [InlineData("test@example.com", "Password123!", "Full Name", "invalid-phone", UserRole.Candidate, false)]
        [InlineData("test@example.com", "Password123!", "Full Name", "+1234567890", UserRole.None, false)]
        public void RegisterDtoValidator_ValidatesCorrectly(string email, string password, string fullName, string mobile, UserRole role, bool expectedValid)
        {
            // Arrange
            var dto = new RegisterDto
            {
                Email = email,
                Password = password,
                FullName = fullName,
                MobileNumber = mobile,
                Role = role
            };

            // Act
            var result = _registerValidator.Validate(dto);

            // Assert
            Assert.Equal(expectedValid, result.IsValid);
        }

        [Theory]
        [InlineData("test@example.com", "Password123!", true)]
        [InlineData("test@example.com", "short", false)] // Password < 8
        [InlineData("invalid-email", "Password123!", false)]
        public void LoginDtoValidator_ValidatesCorrectly(string email, string password, bool expectedValid)
        {
            // Arrange
            var dto = new LoginDto
            {
                Email = email,
                Password = password
            };

            // Act
            var result = _loginValidator.Validate(dto);

            // Assert
            Assert.Equal(expectedValid, result.IsValid);
        }
    }
}

