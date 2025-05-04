using Fly.Flight.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fly.Flight.Infrastructure.DbContext
{
    public class FlightDbContext:Microsoft.EntityFrameworkCore.DbContext
    {
        public FlightDbContext(DbContextOptions<FlightDbContext> options)
            : base(options)
        {
        }

        public DbSet<Flights> Flights { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Flights>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FlightNumber).HasMaxLength(30).IsRequired();
                entity.Property(e => e.Departure).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Destination).HasMaxLength(100).IsRequired();
                entity.Property(e => e.DepartureTime).IsRequired();
                entity.Property(e => e.Price).IsRequired();
                entity.Property(e => e.PopularityScore).IsRequired();
            });
        }
    }
}
