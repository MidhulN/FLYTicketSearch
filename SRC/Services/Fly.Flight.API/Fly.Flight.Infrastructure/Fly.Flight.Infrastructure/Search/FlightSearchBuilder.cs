using Fly.Flight.Domain.Entities;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace Fly.Flight.Infrastructure.Search
{
    public class FlightSearchBuilder
    {
        private readonly List<QueryContainer> _queries = new List<QueryContainer>();
        private string _sortField = JsonNamingPolicy.CamelCase.ConvertName(nameof(Flights.PopularityScore));
        private SortOrder _sortOrder = SortOrder.Descending;
        private int _page = 1;
        private int _pageSize = 50;

        // Full text search across all fields
        public FlightSearchBuilder WithSearchText(string searchText)
        {
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                _queries.Add(new MultiMatchQuery
                {
                    Fields = new[] { $"{JsonNamingPolicy.CamelCase.ConvertName(nameof(Flights.FlightNumber))}^3",
                        $"{JsonNamingPolicy.CamelCase.ConvertName(nameof(Flights.Destination))}^2",
                        $"{JsonNamingPolicy.CamelCase.ConvertName(nameof(Flights.Departure))}^2" },
                    Query = searchText
                });
            }
            return this;
        }

        public FlightSearchBuilder SortByPopularity(bool ascending = false)
        {
            _sortField = JsonNamingPolicy.CamelCase.ConvertName(nameof(Flights.PopularityScore));
            _sortOrder = ascending ? SortOrder.Ascending : SortOrder.Descending;
            return this;
        }

        public FlightSearchBuilder SortByPrice(bool ascending = true)
        {
            _sortField = JsonNamingPolicy.CamelCase.ConvertName(nameof(Flights.Price));
            _sortOrder = ascending ? SortOrder.Ascending : SortOrder.Descending;
            return this;
        }

        public FlightSearchBuilder SortByDepartureTime(bool ascending = true)
        {
            _sortField = JsonNamingPolicy.CamelCase.ConvertName(nameof(Flights.DepartureTime));
            _sortOrder = ascending ? SortOrder.Ascending : SortOrder.Descending;
            return this;
        }

        public FlightSearchBuilder WithPage(int page)
        {
            if (page > 0)
                _page = page;
            return this;
        }

        public FlightSearchBuilder WithPageSize(int pageSize)
        {
            if (pageSize > 0)
                _pageSize = pageSize;
            return this;
        }

        public FlightSearchBuilder WithPriceRange(decimal? minPrice, decimal? maxPrice)
        {
            if (minPrice.HasValue || maxPrice.HasValue)
            {
                _queries.Add(new NumericRangeQuery
                {
                    Field = JsonNamingPolicy.CamelCase.ConvertName(nameof(Flights.Price)),
                    GreaterThanOrEqualTo = (double)minPrice,
                    LessThanOrEqualTo = (double)maxPrice
                });
            }
            return this;
        }

        public FlightSearchBuilder WithDeparture(string departure)
        {
            if (!string.IsNullOrWhiteSpace(departure))
            {
                _queries.Add(new TermQuery
                {
                    Field = $"{JsonNamingPolicy.CamelCase.ConvertName(nameof(Flights.Departure))}.keyword",
                    Value = departure,
                    CaseInsensitive=true
                });
            }
            return this;
        }

        public FlightSearchBuilder WithDestination(string destination)
        {
            if (!string.IsNullOrWhiteSpace(destination))
            {
                _queries.Add(new TermQuery
                {
                    Field = $"{JsonNamingPolicy.CamelCase.ConvertName(nameof(Flights.Destination))}.keyword",
                    Value = destination,
                    CaseInsensitive=true
                });
            }
            return this;
        }

        public FlightSearchBuilder WithDepartureDateRange(DateTime? startDate, DateTime? endDate)
        {
            if (startDate.HasValue || endDate.HasValue)
            {
                _queries.Add(new DateRangeQuery
                {
                    Field = JsonNamingPolicy.CamelCase.ConvertName(nameof(Flights.DepartureTime)),
                    GreaterThanOrEqualTo = startDate,
                    LessThanOrEqualTo = endDate
                });
            }
            return this;
        }
        public SearchRequest<Flights> Build()
        {
            QueryContainer query;

            if (_queries.Count == 0)
            {
                query = new MatchAllQuery();
            }
            else if (_queries.Count == 1)
            {
                query = _queries[0];
            }
            else
            {
                query = new BoolQuery { Must = _queries };
            }

            return new SearchRequest<Flights>(nameof(Flights).ToLower())
            {
                From = (_page - 1) * _pageSize,
                Size = _pageSize,
                Query = query,
                Sort = new List<ISort>
                {
                    new FieldSort { Field = _sortField, Order = _sortOrder }
                }
            };
        }
    }
}
