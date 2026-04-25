using EventTicketingAiPlatform.Infrastructure.InMemory;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventTicketingAiPlatform.Api.Tests
{
    public sealed class TestWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                var storeDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(InMemoryStore));

                if (storeDescriptor is not null)
                    services.Remove(storeDescriptor);

                services.AddSingleton(sp =>
                {
                    var store = new InMemoryStore();
                    InMemorySeed.Seed(store);
                    return store;
                });
            });
        }
    }
}
