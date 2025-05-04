using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fly.Flight.Domain.DTOs
{
    public class SearchFlightsDTO
    {
        public string query { get; set; }
        public string sortBy { get; set; }
        public int page { get; set; }
        public int size { get; set; } 
        public string sortDirection { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string Departure { get; set; }
        public string Destination { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
