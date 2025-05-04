using Fly.Flight.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fly.Flight.Domain.Interfaces
{
    public interface IFlightRepository
    {
        Task<IEnumerable<Flights>> GetFlightsAsync();
        Task<Flights> GetFlightByIdAsync(int id);
    }
}
