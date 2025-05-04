using AutoMapper;
using Fly.Flight.Application.Queries;
using Fly.Flight.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fly.Flight.Application.Mappings
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<SearchFlightsQuery, SearchFlightsDTO>();
        }
    }
}
