using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Velora.Core.AutoMapper;
using Velora.Core.Entities;
using Velora.Core.Interfaces;
using Velora.Infrastructure.Data;
using Velora.Infrastructure.Repositories;
using Velora.Infrastructure.Repositories.AdoNet;
using Velora.Infrastructure.Repositories.Dapper;
using Velora.Infrastructure.Repositories.EfCore;
using Velora.Infrastructure.Services;

namespace Velora.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMyAppServices(this IServiceCollection services)
        {
            services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());
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

            // SystemUser repositories
            services.AddScoped<EfCoreSystemUserRepository>();
            services.AddScoped<AdoSystemUserRepository>();
            services.AddScoped<DapperSystemUserRepository>();

            services.AddScoped<ISystemUserRepository>(sp =>
                new SystemUserRepository(
                    new ISystemUserRepository[]
                    {
                        sp.GetRequiredService<EfCoreSystemUserRepository>(),
                        sp.GetRequiredService<AdoSystemUserRepository>(),
                        sp.GetRequiredService<DapperSystemUserRepository>()
                    },
                    sp.GetRequiredService<IDataProviderService>()
                ));

            // Admin repositories
            services.AddScoped<EfCoreAdminRepository>();
            services.AddScoped<AdoNetAdminRepository>();
            services.AddScoped<DapperAdminRepository>();

            services.AddScoped<IAdminRepository>(sp =>
                new AdminRepository(
                    new IAdminRepository[]
                    {
                        sp.GetRequiredService<EfCoreAdminRepository>(),
                        sp.GetRequiredService<AdoNetAdminRepository>(),
                        sp.GetRequiredService<DapperAdminRepository>()
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
        public static IApplicationBuilder SeedRoles(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var roles = new[] { "Admin", "Recruiter", "Candidate" };
                foreach (var role in roles)
                {
                    if (!roleManager.RoleExistsAsync(role).Result)
                    {
                        roleManager.CreateAsync(new IdentityRole(role)).Wait();
                    }
                }
            }
            return app;
        }
    }

}

