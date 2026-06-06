using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using SmartHire.API.Extensions;
using SmartHire.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureSerilog(builder.Configuration);
builder.Services.AddDatabaseContext(builder.Configuration);
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

app.SeedRoles();

app.UseExceptionHandling();
app.UseSwaggerIfDevelopment();  
app.UseHttpsRedirection();

app.UseRouting();
app.UseSerilogLogging();
app.UseConfiguredCors(app.Configuration);

app.UseAuthentication();
app.UseAuthorization();

await app.ApplyMigrationsIfNotTestingAsync();

app.MapControllers();
app.MapHealthChecks("/health");
app.Run();

public partial class Program { }
