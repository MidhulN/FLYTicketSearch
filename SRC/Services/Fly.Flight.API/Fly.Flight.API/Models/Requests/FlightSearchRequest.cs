using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fly.Flight.API.Models.Requests
{
    public class FlightSearchRequest
    {
        public string SearchQuery { get; set; }
        public string SortBy { get; set; } = "departuretime";
        public string SortDirection { get; set; } = "asc";
        public int Page { get; set; } = 0;
        public int Size { get; set; } = 50;
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string Departure { get; set; }
        public string Destination { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
