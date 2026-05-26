using Microsoft.EntityFrameworkCore;
using SmartHire.Infrastructure.Data;

namespace SmartHire.API.Extensions
{
    public static class DbContextExtension
    {
        public static IServiceCollection AddDatabaseContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            return services;
        }
    }
}
