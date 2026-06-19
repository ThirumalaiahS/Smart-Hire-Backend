using Microsoft.EntityFrameworkCore;
using SmartHire.API.Middlewares;
using SmartHire.Infrastructure.Data;

namespace SmartHire.API.Extensions
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionHandling>();
        }

        public static async Task ApplyMigrationsIfNotTestingAsync(this WebApplication app)
        {
            if (!app.Environment.IsEnvironment("Testing"))
            {
                using (var scope = app.Services.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    await db.Database.MigrateAsync();
                }
            }
        }
        
        public static IApplicationBuilder UseSwaggerIfDevelopment(this IApplicationBuilder app)
        {
            if (app.ApplicationServices.GetRequiredService<IHostEnvironment>().IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                });
            }
            return app;
        }

        public static IApplicationBuilder UseConfiguredCors(this IApplicationBuilder app, IConfiguration configuration)
        {
            var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();
            app.UseCors(options =>
                options.WithOrigins(allowedOrigins ?? Array.Empty<string>())
                       .AllowAnyHeader()
                       .AllowAnyMethod());
            return app;
        }
    }
}
