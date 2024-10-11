using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using TicketSystem.Repository;
using TicketSystem.Interfaces;

namespace TicketSystem
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new HostBuilder()
                .ConfigureServices((context, services) =>
                {
                    var filePath = "path/to/tickets.json";
                    services.AddSingleton<ITicketRepository>(provider => new TicketRepository(filePath));
                })
                .Build();

            host.Run();
        }
    }
}