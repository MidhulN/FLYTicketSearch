using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fly.Flight.Domain.Entities
{
    public class Flights
    {
        public Guid Id { get; set; }
        public string FlightNumber { get; set; }
        public string Departure { get; set; }
        public string Destination { get; set; }
        public DateTime DepartureTime { get; set; }
        public decimal Price { get; set; }
        public int PopularityScore { get; set; }
    }
}
