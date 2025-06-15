using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherForecast.Domain.Models;

namespace WeatherForecast.Application.Interfaces
{
    public interface IWeatherService
    {
        Task<string> GetForecastAsync(double lat, double lon);
        Task<Location> GetLocationByIdAsync(int id);
        Task<string> GetForecastByIdAsync(int id);
        Task AddLocationAsync(double lat, double lon);
        Task<List<Location>> GetLocationsAsync();
        Task DeleteLocationAsync(Location location);
    }
}
