using WeatherForecast.Domain.Models;

namespace WeatherForecast.Application.Interfaces
{
    public interface ILocationRepository
    {
        Task<Location> GetOrCreateAsync(double latitude, double longitude);
        Task<List<Location>> GetAllAsync();
        Task<Location?> GetByCoordinatesAsync(double latitude, double longitude);
        Task<Location?> GetByIdAsync(int id);
        Task AddAsync(double latitude, double longitude);
        Task DeleteAsync(Location location);
        Task HandleForecastsForGivenLocation(int id);
    }
}
