using FluentValidation;
using FluentValidation.AspNetCore;
using Velora.Core.DTOs;

namespace Velora.API.Extensions
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

