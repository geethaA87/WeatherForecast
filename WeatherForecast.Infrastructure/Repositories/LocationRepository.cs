using Microsoft.EntityFrameworkCore;
using WeatherForecast.Application.Interfaces;
using WeatherForecast.Domain.Models;
using WeatherForecast.Infrastructure.Data;

namespace WeatherForecast.Infrastructure.Repositories
{
    public class LocationRepository : ILocationRepository
    {
        private readonly WeatherDbContext _context;
        private readonly IWeatherForecastRepository _forecastRepo;

        public LocationRepository(WeatherDbContext context, IWeatherForecastRepository forecastRepo)
        {
            _context = context;
            _forecastRepo = forecastRepo;
        }

        public async Task<Location> GetOrCreateAsync(double latitude, double longitude)
        {
            var loc = await GetByCoordinatesAsync(latitude, longitude);

            if (loc != null)
            {
                await HandleForecastsForGivenLocation(loc.Id);
                return loc;
            }
            loc = new Location { Latitude = latitude, Longitude = longitude, CreatedDate = DateTime.Now };
            _context.Locations.Add(loc);
            await _context.SaveChangesAsync();
            return loc;
        }

        public async Task<List<Location>> GetAllAsync()
        {
            return await _context.Locations.ToListAsync();
        }

        public async Task<Location?> GetByCoordinatesAsync(double latitude, double longitude)
        {
            var loc = await _context.Locations
                .FirstOrDefaultAsync(l => l.Latitude == latitude && l.Longitude == longitude);

            return loc;
        }

        public async Task<Location?> GetByIdAsync(int id)
        {
            var loc = await _context.Locations
                .FirstOrDefaultAsync(l => l.Id == id);

            return loc;
        }

        public async Task AddAsync(double latitude, double longitude)
        {
            var location = new Location { Latitude = latitude, Longitude = longitude, CreatedDate = DateTime.Now };
            _context.Locations.Add(location);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Location location)
        {
            _context.Locations.Remove(location);
            await _context.SaveChangesAsync();
        }

        public async Task HandleForecastsForGivenLocation(int id)
        {
            var forecasts = await _forecastRepo.GetByLocationAsync(id);
            if (forecasts?.Any() == true)
            {
                await _forecastRepo.DeleteLocationForecastAsync(id); // Deleteing existing data to avoid unnecessary history storage
            }
        }
        
    }
}
