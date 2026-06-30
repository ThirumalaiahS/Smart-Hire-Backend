using Microsoft.EntityFrameworkCore;
using Velora.API.Extensions;
using Velora.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureSerilog(builder.Configuration);

if (builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseInMemoryDatabase("InMemoryAppDbForTesting"));
}
else
{
    builder.Services.AddDatabaseContext(builder.Configuration);
}

builder.Services.AddMyAppServices();
builder.Services.AddIdentityService();
builder.Services.AddControllers();
builder.Services.AddValidatorService();
builder.Services.AddHealthChecks();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger();
builder.Services.AddAuth(builder.Configuration);
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseExceptionHandling();
app.UseSwaggerIfDevelopment();  
app.UseHttpsRedirection();

app.UseRouting();
app.UseSerilogLogging();
app.UseConfiguredCors(app.Configuration);

app.UseAuthentication();
app.UseAuthorization();

await app.ApplyMigrationsIfNotTestingAsync();
await app.SeedRoles();

app.MapControllers();
app.MapHealthChecks("/health");
app.Run();

public partial class Program { }

