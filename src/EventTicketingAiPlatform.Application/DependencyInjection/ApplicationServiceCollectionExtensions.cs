using EventTicketingAiPlatform.Application.Agent;
using EventTicketingAiPlatform.Application.Risk;
using EventTicketingAiPlatform.Application.UseCases.Agent;
using EventTicketingAiPlatform.Application.UseCases.Dashboard;
using EventTicketingAiPlatform.Application.UseCases.Risk;
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

            // services.AddScoped<IRiskExplanationService, RuleBasedRiskExplanationService>();
            services.AddScoped<GetScanRiskAssessmentHandler>();

            services.AddScoped<GetDashboardSummaryHandler>();

            services.AddScoped<IAntifraudAgent, RuleBasedAntifraudAgent>();
            services.AddScoped<AnalyzeScanWithAgentHandler>();

            return services;
        }
    }
}
