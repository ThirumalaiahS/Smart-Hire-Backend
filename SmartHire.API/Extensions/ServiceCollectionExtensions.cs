using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SmartHire.Core.Entities;
using SmartHire.Core.Interfaces;
using SmartHire.Infrastructure.Data;
using SmartHire.Infrastructure.Repositories;
using SmartHire.Infrastructure.Repositories.AdoNet;
using SmartHire.Infrastructure.Repositories.Dapper;
using SmartHire.Infrastructure.Repositories.EfCore;
using SmartHire.Infrastructure.Services;
using System.Text;

namespace SmartHire.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMyAppServices(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<IUserContextService, UserContextService>();
            services.AddScoped<IDataProviderService, DataProviderService>();
            services.AddScoped<ITokenService, TokenService>();

            // Register specific implementations
            services.AddScoped<EfCoreJobApplicationRepository>();
            services.AddScoped<DapperJobApplicationRepository>();
            services.AddScoped<AdoNetJobApplicationRepository>();

            // Register the dispatcher as the primary implementation
            services.AddScoped<IJobApplicationRepository>(sp => 
                new JobApplicationRepository(
                    new IJobApplicationRepository[] 
                    {
                        sp.GetRequiredService<EfCoreJobApplicationRepository>(),
                        sp.GetRequiredService<DapperJobApplicationRepository>(),
                        sp.GetRequiredService<AdoNetJobApplicationRepository>()
                    },
                    sp.GetRequiredService<IDataProviderService>()
                ));

            // Dashboard repositories
            services.AddScoped<EfCoreDashboardRepository>();
            services.AddScoped<DapperDashboardRepository>();
            services.AddScoped<AdoNetDashboardRepository>();

            services.AddScoped<IDashboardRepository>(sp => 
                new DashboardRepository(
                    new IDashboardRepository[] 
                    {
                        sp.GetRequiredService<EfCoreDashboardRepository>(),
                        sp.GetRequiredService<DapperDashboardRepository>(),
                        sp.GetRequiredService<AdoNetDashboardRepository>()
                    },
                    sp.GetRequiredService<IDataProviderService>()
                ));

            // Log repositories
            services.AddScoped<EfCoreLogRepository>();
            services.AddScoped<DapperLogRepository>();
            services.AddScoped<AdoNetLogRepository>();

            services.AddScoped<ILogRepository>(sp =>
                new LogRepository(
                    new ILogRepository[]
                    {
                        sp.GetRequiredService<EfCoreLogRepository>(),
                        sp.GetRequiredService<DapperLogRepository>(),
                        sp.GetRequiredService<AdoNetLogRepository>()
                    },
                    sp.GetRequiredService<IDataProviderService>()
                ));

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
