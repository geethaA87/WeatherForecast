using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherForecast.Domain.Models;

namespace WeatherForecast.Application.Interfaces
{
    public interface IWeatherForecastRepository
    {
        Task<List<WeatherForecastRecord>> GetByLocationAsync(int locationId);
        Task AddAsync(WeatherForecastRecord record);
        Task DeleteLocationForecastAsync(int locationId);
    }
}
