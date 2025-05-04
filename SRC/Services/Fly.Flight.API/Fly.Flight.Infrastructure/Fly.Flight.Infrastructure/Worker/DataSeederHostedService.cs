using Fly.Flight.Infrastructure.Migration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Fly.Flight.Infrastructure.Worker
{
    public class DataSeederHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        //private readonly ILogger<DataSeederHostedService> _logger;

        public DataSeederHostedService(
            IServiceProvider serviceProvider
            //,ILogger<DataSeederHostedService> logger
            )
        {
            _serviceProvider = serviceProvider;
           // _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            //_logger.LogInformation("Starting data seeding...");

            using var scope = _serviceProvider.CreateScope();
            var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
            await seeder.SeedDataAsync();

           // _logger.LogInformation("Data seeding completed");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
