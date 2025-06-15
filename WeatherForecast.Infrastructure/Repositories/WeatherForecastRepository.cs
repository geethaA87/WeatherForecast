using Microsoft.EntityFrameworkCore;
using WeatherForecast.Application.Interfaces;
using WeatherForecast.Domain.Models;
using WeatherForecast.Infrastructure.Data;

namespace WeatherForecast.Infrastructure.Repositories
{
    public class WeatherForecastRepository: IWeatherForecastRepository
    {
        private readonly WeatherDbContext _context;

        public WeatherForecastRepository(WeatherDbContext context)
        {
            _context = context;
        }
        public async Task<List<WeatherForecastRecord>> GetByLocationAsync(int locationId)
        {
            var forecasts = await _context.WeatherForecasts
                            .Where(wf => wf.LocationId == locationId)
                            .ToListAsync();
            return forecasts;
        }
        public async Task AddAsync(WeatherForecastRecord record)
        {
            _context.WeatherForecasts.Add(record);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteLocationForecastAsync(int locationId)
        {
            var forecasts = await _context.WeatherForecasts
                            .Where(wf => wf.LocationId == locationId)
                            .ToListAsync();

            _context.WeatherForecasts.RemoveRange(forecasts);
            await _context.SaveChangesAsync();
        }
    }
}
