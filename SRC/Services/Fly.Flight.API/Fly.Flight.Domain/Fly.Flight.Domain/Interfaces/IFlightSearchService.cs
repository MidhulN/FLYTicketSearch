using Fly.Flight.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fly.Flight.Domain.Interfaces
{
    public interface IFlightSearchService
    {
        Task<IEnumerable<Flights>> SearchFlightsAsync(string query, string sortBy, int page, int size);
    }
}
