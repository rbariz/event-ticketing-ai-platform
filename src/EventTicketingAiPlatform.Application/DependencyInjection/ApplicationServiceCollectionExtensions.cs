using EventTicketingAiPlatform.Application.Risk;
using EventTicketingAiPlatform.Application.UseCases.Scans;
using EventTicketingAiPlatform.Application.UseCases.ScanValidation;
using EventTicketingAiPlatform.Application.UseCases.Tickets;
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

            services.AddScoped<GetScanHistoryHandler>();
            services.AddScoped<GetTicketByCodeHandler>();
            services.AddScoped<IRiskScoringService, RuleBasedRiskScoringService>();

            return services;
        }
    }
}
