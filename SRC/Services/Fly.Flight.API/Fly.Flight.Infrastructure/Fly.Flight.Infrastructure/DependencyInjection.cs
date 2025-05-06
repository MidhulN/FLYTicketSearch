using Fly.Flight.Domain.Entities;
using Fly.Flight.Domain.Interfaces;
using Fly.Flight.Infrastructure.DbContext;
using Fly.Flight.Infrastructure.Migration;
using Fly.Flight.Infrastructure.Persistence;
using Fly.Flight.Infrastructure.Search;
using Fly.Flight.Infrastructure.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fly.Flight.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Configure SQL Server
            services.AddDbContext<FlightDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("Default"),
                    sqlOptions => sqlOptions.EnableRetryOnFailure()
                )
            );

            // Configure Elasticsearch
            var elasticsearchUrl = configuration["Elasticsearch:Uri"];
            var settings = new ConnectionSettings(new Uri(elasticsearchUrl))
                .DefaultIndex(nameof(Flights))
                .DefaultMappingFor<Flights>(m => m
                    .IdProperty(f => f.Id)
                );

            // Configure Redis
            var redisUrl = configuration["Redis:Uri"];
            services.AddSingleton<IConnectionMultiplexer>(sp =>
                    ConnectionMultiplexer.Connect(redisUrl));

            services.AddSingleton<IElasticClient>(new ElasticClient(settings));
            services.AddScoped<IElasticsearchFlightRepository, ElasticsearchFlightRepository>();
            services.AddScoped<IRedisCacheService, RedisCacheService>();

            // Register the data seeder
            services.AddTransient<DataSeeder>();


            //services.AddHostedService<DataSeederHostedService>();

            return services;
        }
    }
}
