using Bogus;
using Fly.Flight.Domain.Entities;
using Fly.Flight.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fly.Flight.Infrastructure.Migration
{
    public class DataSeeder
    {
        private readonly FlightDbContext _dbContext;
        private readonly IElasticClient _elasticClient;
        private readonly string IndexName ;

        public DataSeeder(
            FlightDbContext dbContext,
            IElasticClient elasticClient)
        {
            _dbContext = dbContext;
            _elasticClient = elasticClient;
            IndexName = nameof(Flights).ToLower();
        }

        public async Task SeedDataAsync(int numberOfFlights = 100)
        {
            try
            {
                _dbContext.Database.EnsureCreated();
                if (await _dbContext.Flights.AnyAsync())
                {
                   // _logger.LogInformation("Database already seeded. Skipping.");
                }
                else
                {
                    // Generate fake flight data
                    var flights = GenerateFlights(numberOfFlights);

                    
                    await _dbContext.Flights.AddRangeAsync(flights);
                    await _dbContext.SaveChangesAsync();
                    //_logger.LogInformation($"Added {flights.Count} flights to SQL Server");
                }

                // Ensure Elasticsearch index exists
                await EnsureElasticsearchIndexAsync();

                // Get all flights from SQL Server
                var allFlights = await _dbContext.Flights.ToListAsync();

                // Index flights in Elasticsearch
                await IndexFlightsInElasticsearchAsync(allFlights);
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "Error seeding data");
                throw;
            }
        }
        private List<Flights> GenerateFlights(int count)
        {
            var airports = new[]
            {
                "Dubai", "Siberia", "Russia", "China", "Spain", "Singapore", "Malasia", "ATL", "MIA", "BOS",
                "LHR", "CDG", "AMS", "FRA", "MAD", "FCO", "IST", "DXB", "SIN", "HKG"
            };

            var airlines = new[] { "FLY Dubai", "USA Airlines", "Denmark", "Boieng", "Boston", "Arihant", "Emirites", "Siberia", "SpaceX" };

            var faker = new Faker<Flights>()
                .RuleFor(f => f.Id, f => Guid.NewGuid())
                .RuleFor(f => f.FlightNumber, f =>
                    f.PickRandom(airlines) + f.Random.Number(100, 9999).ToString("0000"))
                .RuleFor(f => f.Departure, f => f.PickRandom(airports))
                .RuleFor(f => f.Destination, (f, flight) =>
                {
                    string destination;
                    do
                    {
                        destination = f.PickRandom(airports);
                    } while (destination == flight.Departure);
                    return destination;
                })
                .RuleFor(f => f.DepartureTime, f =>
                    f.Date.Between(DateTime.Now, DateTime.Now.AddMonths(6)))
                .RuleFor(f => f.Price, f => Math.Round(f.Random.Decimal(50, 2000), 2))
                .RuleFor(f => f.PopularityScore, f => f.Random.Int(1, 100));

            return faker.Generate(count);
        }
        private async Task EnsureElasticsearchIndexAsync()
        {
            var indexExists = await _elasticClient.Indices.ExistsAsync(IndexName);

            if (!indexExists.Exists)
            {
               // _logger.LogInformation($"Creating Elasticsearch index '{IndexName}'");

                var createIndexResponse = await _elasticClient.Indices.CreateAsync(IndexName, c => c
                    .Settings(s => s
                        .Analysis(a => a
                            .Analyzers(an => an
                                .Standard("standard_analyzer", sa => sa
                                    .StopWords("_english_")
                                )
                            )
                        )
                    )
                    .Map<Flights>(m => m
                        .AutoMap()
                        .Properties(p => p
                            .Keyword(k => k
                                .Name(n => n.FlightNumber)
                                .Fields(f => f
                                    .Text(t => t
                                        .Name("search")
                                        .Analyzer("standard_analyzer")
                                    )
                                )
                            )
                            .Text(t => t
                                .Name(n => n.Departure)
                                .Fields(f => f
                                    .Keyword(k => k.Name("keyword"))
                                )
                            )
                            .Text(t => t
                                .Name(n => n.Destination)
                                .Fields(f => f
                                    .Keyword(k => k.Name("keyword"))
                                )
                            )
                            .Date(d => d
                                .Name(n => n.DepartureTime)
                            )
                            .Number(n => n
                                .Name(f => f.Price)
                                .Type(NumberType.Double)
                            )
                            .Number(n => n
                                .Name(f => f.PopularityScore)
                                .Type(NumberType.Integer)
                            )
                        )
                    )
                );

                if (!createIndexResponse.IsValid)
                {
                    //_logger.LogError($"Error creating Elasticsearch index: {createIndexResponse.DebugInformation}");
                    throw new Exception("Failed to create Elasticsearch index");
                }
            }
        }

        private async Task IndexFlightsInElasticsearchAsync(List<Flights> flights)
        {
           // _logger.LogInformation($"Indexing {flights.Count} flights in Elasticsearch");

            var bulkDescriptor = new BulkDescriptor();

            foreach (var flight in flights)
            {
                bulkDescriptor.Index<Flights>(i => i
                    .Index(IndexName)
                    .Id(flight.Id.ToString())
                    .Document(flight)
                );
            }

            var bulkResponse = await _elasticClient.BulkAsync(bulkDescriptor);

            if (!bulkResponse.IsValid)
            {
                //_logger.LogError($"Error indexing flights in Elasticsearch: {bulkResponse.DebugInformation}");
                throw new Exception("Failed to index flights in Elasticsearch");
            }

            //_logger.LogInformation("Successfully indexed flights in Elasticsearch");
        }
    }


}
