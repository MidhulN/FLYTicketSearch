using Fly.Flight.Domain.DTOs;
using Fly.Flight.Domain.Entities;
using Fly.Flight.Infrastructure.Search;
using Moq;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Fly.Flight.Test.Fly.Flight.InfrastructureTest
{
    public class ElasticsearchFlightRepositoryTests
    {
        [Fact]
        public async Task Search_ShouldReturnValidResult_WhenElasticClientReturnsData()
        {
            // Arrange
            var mockClient = new Mock<IElasticClient>();
            var flights = new List<Flights>
        {
            new Flights { Id = new System.Guid(), Departure = "NYC", Destination = "LAX", Price = 300 },
            new Flights { Id = new System.Guid(), Departure = "NYC", Destination = "LAX", Price = 400 }
        };

            var mockResponse = new Mock<ISearchResponse<Flights>>();
            mockResponse.Setup(r => r.IsValid).Returns(true);
            mockResponse.Setup(r => r.Documents).Returns(flights);

            mockClient
                .Setup(x => x.SearchAsync<Flights>(
                    It.IsAny<SearchRequest<Flights>>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(mockResponse.Object);

            var repository = new ElasticsearchFlightRepository(mockClient.Object);

            var searchDto = new SearchFlightsDTO
            {
                query = "flight",
                page = 1,
                size = 10,
                Departure = "NYC",
                Destination = "LAX",
                StartDate = null,
                EndDate = null,
                MinPrice = null,
                MaxPrice = null,
                sortBy = "price",
                sortDirection = "asc"
            };

            // Act
            var (isValid, result) = await repository.Search(searchDto);

            // Assert
            Assert.True(isValid);
            Assert.Equal(2, result.Count());
            Assert.Equal("NYC", result.First().Departure);
        }
    }
}
