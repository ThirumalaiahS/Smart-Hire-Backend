using Microsoft.AspNetCore.Identity;
using SmartHire.Core.Entities;
using SmartHire.Core.Interfaces;
using SmartHire.Infrastructure.Data;
using SmartHire.Infrastructure.Repositories;

namespace SmartHire.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMyAppServices(this IServiceCollection services)
        {
            services.AddScoped<IJobApplicationRepository, JobApplicationRepository>();
            services.AddScoped<IDashboardRepository, DashboardRepository>();
            return services;
        }

        public static IServiceCollection AddIdentityService(this IServiceCollection services)
        {
            services.AddIdentity<AppUser, IdentityRole>(o =>
            {
                o.Password.RequireDigit = true; 
                o.Password.RequiredLength = 8;
                o.Password.RequireNonAlphanumeric = false;
            })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            return services;
        }
    }

}
