using Serilog;

namespace SmartHire.API.Extensions
{
    public static class SerilogExtensions
    {
        public static void ConfigureSerilog(this IHostBuilder hostBuilder, IConfiguration configuration)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            hostBuilder.UseSerilog();
        }

        public static IApplicationBuilder UseSerilogLogging(this IApplicationBuilder app)
        {
            return app.UseSerilogRequestLogging();
        }
    }
}
