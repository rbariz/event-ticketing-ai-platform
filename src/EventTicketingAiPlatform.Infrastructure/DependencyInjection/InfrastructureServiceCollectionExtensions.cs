using EventTicketingAiPlatform.Application.Abstractions;
using EventTicketingAiPlatform.Infrastructure.InMemory;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventTicketingAiPlatform.Infrastructure.DependencyInjection
{
    public static class InfrastructureServiceCollectionExtensions
    {
        public static IServiceCollection AddInMemoryInfrastructure(this IServiceCollection services)
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

            

            return services;
        }
    }
}
