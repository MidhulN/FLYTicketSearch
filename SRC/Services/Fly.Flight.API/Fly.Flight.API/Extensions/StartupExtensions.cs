using Fly.Flight.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fly.Flight.Infrastructure.DbContext;
using Fly.Flight.Infrastructure.Migration;
using Fly.Flight.Infrastructure.Worker;
using Fly.Flight.Infrastructure;
using Fly.Flight.Application;

namespace Fly.Flight.API.Extensions
{
    public class StartupExtensions
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddInfrastructure(configuration);
            services.AddApplication();


        }
    }
}
