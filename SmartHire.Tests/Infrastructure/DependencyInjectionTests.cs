using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartHire.API.Extensions;
using SmartHire.Core.Interfaces;
using SmartHire.Infrastructure.Data;
using SmartHire.Infrastructure.Repositories;

namespace SmartHire.Tests.Infrastructure
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
                    {"ConnectionStrings:DefaultConnection", "Server=DESKTOP-2EIRK23\\SQLEXPRESS;Database=SmartHireTest;Trusted_Connection=True;MultipleActiveResultSets=true"}
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
            Assert.Null(repo);
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
