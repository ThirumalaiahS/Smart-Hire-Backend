using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Velora.API.Extensions;
using Velora.Core.Interfaces;
using Velora.Infrastructure.Data;
using Velora.Infrastructure.Repositories;

namespace Velora.Tests.Infrastructure
{
    public class DependencyInjectionTests
    {
        private readonly IServiceProvider _serviceProvider;

        public DependencyInjectionTests()
        {
            var services = new ServiceCollection();

            // Setup mock configuration
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    {"ConnectionStrings:DefaultConnection", "Server=DESKTOP-2EIRK23\\SQLEXPRESS;Database=VeloraTestDb;Trusted_Connection=True;TrustServerCertificate=True;"}
                })
                .Build();

            services.AddSingleton<IConfiguration>(configuration);

            // Add DbContext (using In-Memory for tests if possible, or just register it)
            services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("TestDb"));

            // Use the actual registration method from the API
            services.AddMyAppServices();

            _serviceProvider = services.BuildServiceProvider();
        }

        [Fact]
        public void JobApplicationRepository_Should_Be_Resolvable()
        {
            // Act
            var repo = _serviceProvider.GetService<IJobApplicationRepository>();

            // Assert
            Assert.NotNull(repo);
            Assert.IsType<JobApplicationRepository>(repo);
        }

        [Fact]
        public void DashboardRepository_Should_Be_Resolvable()
        {
            // Act
            var repo = _serviceProvider.GetService<IDashboardRepository>();

            // Assert
            Assert.NotNull(repo);
            Assert.IsType<DashboardRepository>(repo);
        }
    }
}

