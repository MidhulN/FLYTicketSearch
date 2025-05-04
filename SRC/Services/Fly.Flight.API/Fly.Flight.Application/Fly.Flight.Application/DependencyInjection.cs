using Fly.Flight.Application.Queries;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Fly.Flight.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<SearchFlightsQuery>());
            services.AddAutoMapper(typeof(DependencyInjection).Assembly);
            return services;
        }
    }
}
