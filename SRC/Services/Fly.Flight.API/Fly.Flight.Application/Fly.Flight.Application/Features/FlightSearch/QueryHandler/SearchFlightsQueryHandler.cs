using AutoMapper;
using Fly.Flight.Application.Queries;
using Fly.Flight.Domain.DTOs;
using Fly.Flight.Domain.Entities;
using Fly.Flight.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Fly.Flight.Application.QueryHandler
{
    public class SearchFlightsQueryHandler : IRequestHandler<SearchFlightsQuery, IEnumerable<Flights>>
    {
        private readonly IElasticsearchFlightRepository _elasticsearchFlightRepository;
        private readonly IRedisCacheService _redisCacheService;
        private readonly IMapper _mapper;

        public SearchFlightsQueryHandler(IElasticsearchFlightRepository elasticsearchFlightRepository, IRedisCacheService redisCacheService,IMapper mapper)
        {
            _elasticsearchFlightRepository = elasticsearchFlightRepository;
            _redisCacheService = redisCacheService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Flights>> Handle(SearchFlightsQuery request, CancellationToken cancellationToken)
        {
            var requestDTO = _mapper.Map<SearchFlightsDTO>(request);
            var cacheKey = $"flights:{request.query}:{request.sortBy}:{request.sortDirection}:{request.page}" +
                $":{request.size}:{request.Destination}:{request.Departure}:{request.EndDate}:" +
                $"{request.MaxPrice}:{request.MinPrice}:{request.StartDate}";

            // if data is avaialble in redis skip
            var cached = await _redisCacheService.GetAsync<IEnumerable<Flights>>(cacheKey);
            if (cached != null) return cached;

            var (isFlightDataValid, flightsResult)= await _elasticsearchFlightRepository.Search(requestDTO);

            if (isFlightDataValid)
            {
                await _redisCacheService.SetAsync(cacheKey, flightsResult, TimeSpan.FromMinutes(2));
            }
            return flightsResult;
        }
    }
}
