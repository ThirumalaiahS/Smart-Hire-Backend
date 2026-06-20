using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SmartHire.Core.DTOs;
using SmartHire.Core.Entities;
using SmartHire.Core.Interfaces;
using System.Net;
using System.Security.Claims;

namespace SmartHire.API.Middlewares
{
    public sealed class ExceptionHandling
    {
        private readonly RequestDelegate _next;

        public ExceptionHandling(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var errorLogRepository = context.RequestServices.GetRequiredService<ILogRepository>();
                var userContextService = context.RequestServices.GetRequiredService<IUserContextService>();

                var userId = userContextService.GetUserId();

                var errorLog = new ErrorLogs
                {
                    UserId = userId,
                    Endpoint = context.Request.Path,
                    HttpMethod = context.Request.Method,
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    ExceptionType = ex.GetType().Name,
                    ErrorMessage = ex.Message,
                    StackTrace = ex.StackTrace,
                    RequestPayload = await ReadRequestBodyAsync(context),
                    ResponsePayload = null,
                    CorrelationId = Guid.NewGuid(),
                    Source = "API",
                    CreatedAt = DateTime.UtcNow,
                    Severity = "Error"
                };

                await errorLogRepository.CreateErrorLogAsync(errorLog);

                var response = ApiResponse<object>.ErrorResponse(
                    new List<string> { ex.Message }, 
                    "An unexpected error occurred. Please try again later.", 
                    (int)HttpStatusCode.InternalServerError);

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsJsonAsync(response);
            }

        }
        private async Task<string> ReadRequestBodyAsync(HttpContext context)
        {
            context.Request.EnableBuffering();
            using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;
            return body;
        }
    }
}
