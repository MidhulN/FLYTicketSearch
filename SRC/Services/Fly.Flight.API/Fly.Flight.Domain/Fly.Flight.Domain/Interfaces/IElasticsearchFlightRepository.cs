using Fly.Flight.Domain.DTOs;
using Fly.Flight.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fly.Flight.Domain.Interfaces
{
    public interface IElasticsearchFlightRepository
    {
        Task<(bool isValid, IEnumerable<Flights>)> Search(SearchFlightsDTO searchFlightsDTO);
    }
}
