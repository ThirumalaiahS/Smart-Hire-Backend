using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SmartHire.Infrastructure.Data;

namespace SmartHire.Tests.Infrastructure
{
    public class StartupTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public StartupTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");
            });
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
            var response = await client.GetAsync("/health");
            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/plain",
                response.Content.Headers.ContentType?.MediaType);
        }
    }
}
