using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace SmartHire.Tests.Infrastructure
{
    public class StartupTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        public StartupTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }
        [Fact]
        public async Task Application_Should_Start_Successfully()
        {
            // Arrange
            var client = _factory.CreateClient();
            // Act
            var response = await client.GetAsync("/health");
            // Assert
            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task Get_EndpointsReturnSuccessAndCorrectContentType()
        {
            // Arrange
            var client = _factory.CreateClient();
            // Act
            var response = await client.GetAsync("/api/auth");
            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType?.ToString());
        }
    }
}
