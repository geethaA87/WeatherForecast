using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using WeatherForecast.Application.Interfaces;
using WeatherForecast.Domain.Models;

namespace WeatherForecast.Application.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly ILogger<WeatherService> _logger;
        private readonly IWeatherForecastRepository _forecastRepo;
        private readonly ILocationRepository _locationRepo;

        public WeatherService(HttpClient httpClient, IMemoryCache cache, ILogger<WeatherService> logger,
                                IWeatherForecastRepository forecastRepo, ILocationRepository locationRepo)
        {
            _httpClient = httpClient;
            _cache = cache;
            _logger = logger;
            _forecastRepo = forecastRepo;
            _locationRepo = locationRepo;
        }

        public async Task<string> GetForecastAsync(double lat, double lon)
        {
            /*Thought of using Cache. But, felt weather forecast needs to be current rather than cached.
            var key = $"{lat}_{lon}";

            if (_cache.TryGetValue(key, out string cached))
            {
                _logger.LogInformation("Returning cached forecast for {lat}, {lon}", lat, lon);
                return cached;
            }
            _cache.Set(key, response, TimeSpan.FromHours(1));
            */

            if (!IsValidCoordinates(lat,lon))
            {
                throw new ArgumentException("Invalid latitude or longitude values.");
            }

            try
            {
                var location = await _locationRepo.GetOrCreateAsync(lat, lon);
                return await GetandStoreForecastData(location.Id, location.Latitude, location.Longitude);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the weather forecast.");
                throw;
            }
        }

        public async Task<string> GetForecastByIdAsync(int id)
        {
            var loc = await _locationRepo.GetByIdAsync(id);
            if (loc == null)
            {
                throw new ArgumentException("Location does not exist.");
            }
            return await GetandStoreForecastData(loc.Id, loc.Latitude, loc.Longitude);
        }

        public async Task<Location> GetLocationByIdAsync(int id)
        {
            try
            {
                var loc = await _locationRepo.GetByIdAsync(id);
                if (loc == null)
                {
                    throw new ArgumentException("Location does not exist.");
                }
                return loc;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fectching location by Id.");
                throw;
            }
        }

        public async Task AddLocationAsync(double lat, double lon)
        {
            if (!IsValidCoordinates(lat, lon))
            {
                throw new ArgumentException("Invalid latitude or longitude values.");
            }
            var loc = await _locationRepo.GetByCoordinatesAsync(lat, lon);
            if (loc != null)
            {
                throw new ArgumentException("Coordinates already exists.");
            }
            try
            {
                await _locationRepo.AddAsync(lat, lon);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding location.");
                throw;
            }

        }

        public async Task<List<Location>> GetLocationsAsync()
        {
            try
            {
                var locations = await _locationRepo.GetAllAsync();

                return locations;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fecthing locations.");
                throw;
            }
        }

        public async Task DeleteLocationAsync(Location location)
        {
            try
            {
                await _locationRepo.DeleteAsync(location);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting location.");
                throw;
            }
        }

        public async Task<string> GetandStoreForecastData(int id,double lat, double lon)
        {
            string url = $"https://api.open-meteo.com/v1/forecast?latitude={lat}&longitude={lon}&hourly=temperature_2m";
            var response = await _httpClient.GetStringAsync(url);

            using var doc = JsonDocument.Parse(response);
            var times = doc.RootElement.GetProperty("hourly").GetProperty("time").EnumerateArray().ToArray();
            var temps = doc.RootElement.GetProperty("hourly").GetProperty("temperature_2m").EnumerateArray().ToArray();

            for (int i = 0; i < Math.Min(24, times.Length); i++)
            {
                var record = new WeatherForecastRecord
                {
                    LocationId = id,
                    Timestamp = DateTime.Parse(times[i].GetString()!),
                    Temperature = temps[i].GetDouble(),
                    CreatedDate = DateTime.Now,
                };
                await _forecastRepo.AddAsync(record);
            }

            return response;
        }

        private bool IsValidCoordinates(double latitude, double longitude)
        {
            return latitude >= -90.0 && latitude <= 90.0 && longitude >= -180.0 && longitude <= 180.0;
        }
    }

}
