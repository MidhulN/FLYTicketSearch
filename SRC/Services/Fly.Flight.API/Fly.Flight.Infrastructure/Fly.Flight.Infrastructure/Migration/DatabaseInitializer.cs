using Fly.Flight.Infrastructure.DbContext;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fly.Flight.Infrastructure.Migration
{
    public static class DatabaseInitializer
    {
        public static async Task EnsureDatabaseAndSeed(IServiceProvider services)
        {
            var retryPolicy = Policy
                .Handle<SqlException>()
                .WaitAndRetryAsync(
                    retryCount: 10,
                    sleepDurationProvider: attempt => TimeSpan.FromSeconds(5),
                    onRetry: (ex, time, retryCount, _) =>
                    {
                        
                    });

            await retryPolicy.ExecuteAsync(async () =>
            {
                using var scope = services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<FlightDbContext>();

                
                

                // Run your seeder
                var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
                await seeder.SeedDataAsync();
            });
        }
    }
}
