using Fly.Flight.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fly.Flight.Application.Queries
{
    public class SearchFlightsQuery: IRequest<IEnumerable<Flights>>
    {
        public string query { get; set; }
        public string sortBy { get; set; } = "DepartureTime";
        public int page { get; set; }
        public int size { get; set; } = 10;
        public string sortDirection { get; set; } = "asc";
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string Departure { get; set; }
        public string Destination { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
