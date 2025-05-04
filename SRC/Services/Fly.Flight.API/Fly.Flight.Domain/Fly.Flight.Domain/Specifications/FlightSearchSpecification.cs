using Fly.Flight.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Fly.Flight.Domain.Specifications
{
    public class FlightSearchSpecification:BaseSpecification<Flights>
    {
        public FlightSearchSpecification(
            string searchQuery,
            int popularity,
            DateTime? departureDate,
            decimal? maxPrice,
            string sortBy,
            bool sortAscending,
            int page = 1,
            int pageSize = 50) : base(BuildSearchCriteria(searchQuery, departureDate, maxPrice, popularity))
        {
            ApplyPaging((page - 1) * pageSize, pageSize);
            switch (sortBy?.ToLower())
            {
                case nameof(Flights.Price):
                    if (sortAscending)
                        ApplyOrderBy(f => f.Price);
                    else
                        ApplyOrderByDescending(f => f.Price);
                    break;
                case nameof(Flights.Departure):
                    if (sortAscending)
                        ApplyOrderBy(f => f.DepartureTime);
                    else
                        ApplyOrderByDescending(f => f.DepartureTime);
                    break;
                
                default:
                    
                    ApplyOrderBy(f => f.DepartureTime);
                    break;
            }
        }

        private static Expression<Func<Flights, bool>> BuildSearchCriteria(
            string searchQuery,
            DateTime? departureDate,
            decimal? maxPrice,
            int? popularity)
        {
            return f =>
                (string.IsNullOrEmpty(searchQuery) ||
                 f.FlightNumber.Contains(searchQuery) ||
                 f.Departure.Contains(searchQuery) ||
                 f.Destination.Contains(searchQuery)) &&
                (!departureDate.HasValue ||
                 f.DepartureTime.Date == departureDate.Value.Date) &&
                (!maxPrice.HasValue ||
                 f.Price <= maxPrice.Value) &&
                (!popularity.HasValue ||
                 f.PopularityScore >= popularity.Value);
        }
    }
}
