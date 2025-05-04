using Fly.Flight.Domain.DTOs;
using Fly.Flight.Domain.Entities;
using Fly.Flight.Domain.Interfaces;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fly.Flight.Infrastructure.Search
{
    public class ElasticsearchFlightRepository : IElasticsearchFlightRepository
    {
        private const string popularity = "popularity";
        private const string price = "price";
        private const string departureTime = "departuretime";
        private readonly IElasticClient _elasticClient;

        public ElasticsearchFlightRepository(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        public async Task<(bool,IEnumerable<Flights>)> Search(SearchFlightsDTO searchFlightsDTO)
        {
            var searchRequest = new FlightSearchBuilder().
                WithSearchText(searchFlightsDTO.query)
                .WithPage(searchFlightsDTO.page)
                .WithPageSize(searchFlightsDTO.size)
                .WithDepartureDateRange(searchFlightsDTO.StartDate,searchFlightsDTO.EndDate)
                .WithDeparture(searchFlightsDTO.Departure)
                .WithDestination(searchFlightsDTO.Destination)
                .WithPriceRange(searchFlightsDTO.MinPrice,searchFlightsDTO.MaxPrice);

            var sortToDirection = searchFlightsDTO.sortDirection.ToLower() == "asc" ? true : false;
            switch (searchFlightsDTO.sortBy.ToLower())
            {
                case popularity:
                    searchRequest.SortByPopularity(sortToDirection);
                    break;
                case price:
                    searchRequest.SortByPrice(sortToDirection);
                    break;
                case departureTime:
                    searchRequest.SortByDepartureTime(sortToDirection);
                    break;

                default:
                    searchRequest.SortByDepartureTime(sortToDirection);
                    break;
            }
            var requestBuild=searchRequest.Build();
            var result= await _elasticClient.SearchAsync<Flights>(requestBuild);
            return (result.IsValid, result.Documents.AsEnumerable());
        }
    }
}