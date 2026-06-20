using FluentValidation;
using FluentValidation.AspNetCore;
using SmartHire.Core.DTOs;

namespace SmartHire.API.Extensions
{
    public static class ValidatorServiceExtension
    {
        public static IServiceCollection AddValidatorService(this IServiceCollection services)
        {
            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining<RegisterDtoValidator>();
            services.AddValidatorsFromAssemblyContaining<LoginDtoValidator>();

            return services;
        }
    }
}
