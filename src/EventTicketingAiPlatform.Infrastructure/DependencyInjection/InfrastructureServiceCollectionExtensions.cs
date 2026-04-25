using EventTicketingAiPlatform.Application.Abstractions;
using EventTicketingAiPlatform.Application.Options;
using EventTicketingAiPlatform.Application.Risk;
using EventTicketingAiPlatform.Infrastructure.AI;
using EventTicketingAiPlatform.Infrastructure.InMemory;
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
        public static IServiceCollection AddInMemoryInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            
            services.AddSingleton<InMemoryStore>(sp =>
            {
                var store = new InMemoryStore();
                InMemorySeed.Seed(store);
                return store;
            });

            services.AddScoped<ITicketRepository, InMemoryTicketRepository>();
            services.AddScoped<IScanAttemptRepository, InMemoryScanAttemptRepository>();
            services.AddScoped<IUnitOfWork, InMemoryUnitOfWork>();

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
