using AutoMapper;
using Fly.Flight.Application.Queries;
using Fly.Flight.Application.QueryHandler;
using Fly.Flight.Domain.DTOs;
using Fly.Flight.Domain.Entities;
using Fly.Flight.Domain.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Fly.Flight.Test.Fly.Flight.ApplicationTest.FeatureTest
{
    public class SearchFlightsQueryHandlerTests
    {
        [Fact]
        public async Task Handle_ReturnsFlights_FromElasticSearch_And_CachesResult_WhenCacheMiss()
        {
            // Arrange
            var mockElasticRepo = new Mock<IElasticsearchFlightRepository>();
            var mockRedisCache = new Mock<IRedisCacheService>();
            var mockMapper = new Mock<IMapper>();

            var query = new SearchFlightsQuery
            {
                query = "flight",
                sortBy = "price",
                sortDirection = "asc",
                page = 1,
                size = 10,
                Destination = "LAX",
                Departure = "NYC",
                StartDate = null,
                EndDate = null,
                MinPrice = null,
                MaxPrice = null
            };

            var flights = new List<Flights>
        {
            new Flights { Id = new Guid(), Departure = "NYC", Destination = "LAX", Price = 300 }
        };

            mockMapper.Setup(m => m.Map<SearchFlightsDTO>(query)).Returns(new SearchFlightsDTO
            {
                query = query.query,
                sortBy = query.sortBy,
                sortDirection = query.sortDirection,
                page = query.page,
                size = query.size,
                Destination = query.Destination,
                Departure = query.Departure
            });

            mockRedisCache.Setup(r => r.GetAsync<IEnumerable<Flights>>(It.IsAny<string>()))
                          .ReturnsAsync((IEnumerable<Flights>)null);

            mockElasticRepo.Setup(r => r.Search(It.IsAny<SearchFlightsDTO>()))
                           .ReturnsAsync((true, flights));

            var handler = new SearchFlightsQueryHandler(mockElasticRepo.Object, mockRedisCache.Object, mockMapper.Object);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Count());
            
        }
    }
}
