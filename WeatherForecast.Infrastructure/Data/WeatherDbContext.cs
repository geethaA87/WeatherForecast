using Microsoft.EntityFrameworkCore;
using WeatherForecast.Domain.Models;

namespace WeatherForecast.Infrastructure.Data
{
    public class WeatherDbContext: DbContext
    {
        public WeatherDbContext(DbContextOptions<WeatherDbContext> options)
        : base(options)
        {
        }
        public DbSet<Location> Locations { get; set; }
        public DbSet<WeatherForecastRecord> WeatherForecasts { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Location>()
                .HasMany(l => l.Forecasts)
                .WithOne(f => f.Location)
                .HasForeignKey(f => f.LocationId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}
