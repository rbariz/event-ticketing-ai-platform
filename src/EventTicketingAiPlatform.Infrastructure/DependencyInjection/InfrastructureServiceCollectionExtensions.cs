using EventTicketingAiPlatform.Application.Abstractions;
using EventTicketingAiPlatform.Application.Options;
using EventTicketingAiPlatform.Application.Risk;
using EventTicketingAiPlatform.Infrastructure.AI;
using EventTicketingAiPlatform.Infrastructure.InMemory;
using EventTicketingAiPlatform.Infrastructure.Persistence;
using EventTicketingAiPlatform.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventTicketingAiPlatform.Infrastructure.DependencyInjection
{
    public static class InfrastructureServiceCollectionExtensions
    {
        public static IServiceCollection AddInMemoryInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddSingleton(sp =>
            {
                var store = new InMemoryStore();
                InMemorySeed.Seed(store);
                return store;
            });

            services.AddScoped<ITicketRepository, InMemoryTicketRepository>();
            services.AddScoped<IScanAttemptRepository, InMemoryScanAttemptRepository>();
            services.AddScoped<IAgentDecisionLogRepository, InMemoryAgentDecisionLogRepository>();
            services.AddScoped<IAgentNotificationRepository, InMemoryAgentNotificationRepository>();
            services.AddScoped<IUnitOfWork, InMemoryUnitOfWork>();

            services.AddRiskExplanation(configuration);

            return services;
        }

        public static IServiceCollection AddPostgresInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration,
            string connectionString)
        {
            services.AddDbContext<EventTicketingDbContext>(options =>
                options.UseNpgsql(connectionString));

            services.AddScoped<ITicketRepository, PgTicketRepository>();
            services.AddScoped<IScanAttemptRepository, PgScanAttemptRepository>();
            services.AddScoped<IAgentDecisionLogRepository, PgAgentDecisionLogRepository>();
            services.AddScoped<IAgentNotificationRepository, PgAgentNotificationRepository>();
            services.AddScoped<IUnitOfWork, PgUnitOfWork>();

            services.AddRiskExplanation(configuration);

            return services;
        }

        private static IServiceCollection AddRiskExplanation(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<OpenAiOptions>(
                configuration.GetSection("OpenAI"));

            services.AddHttpClient<OpenAiRiskExplanationService>();

            services.AddScoped<IRiskExplanationService>(sp =>
            {
                var options = sp.GetRequiredService<IOptions<OpenAiOptions>>().Value;

                if (options.Enabled && !string.IsNullOrWhiteSpace(options.ApiKey))
                    return sp.GetRequiredService<OpenAiRiskExplanationService>();

                return new RuleBasedRiskExplanationService();
            });

            return services;
        }
    }
}
