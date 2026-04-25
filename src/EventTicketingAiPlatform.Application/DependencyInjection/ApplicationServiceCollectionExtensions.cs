using EventTicketingAiPlatform.Application.UseCases.ScanValidation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventTicketingAiPlatform.Application.DependencyInjection
{
    public static class ApplicationServiceCollectionExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<ValidateTicketScanRequestValidator>();
            services.AddScoped<ValidateTicketScanHandler>();

            return services;
        }
    }
}
