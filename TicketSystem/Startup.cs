using Microsoft.Azure.Functions.Extensions.DependencyInjection; // For FunctionsStartup
using Microsoft.Extensions.DependencyInjection;
using TicketSystem.Interfaces;
using TicketSystem.Services; // This ensures the DI container can find TicketService


[assembly: FunctionsStartup(typeof(TicketSystem.Startup))]

namespace TicketSystem
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddSingleton<ITicketService, TicketService>();  // Ensure TicketService is accessible
            // Add other necessary service registrations here
        }
    }
}

