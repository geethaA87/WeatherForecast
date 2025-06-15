using Microsoft.AspNetCore.Mvc;
using WeatherForecast.Application.Interfaces;
using WeatherForecast.Domain.Models;

namespace WeatherForecast.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherController : Controller
    {
        private readonly IWeatherService _weatherService;
        public WeatherController(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        //Gets Forecast by Lat & Long, deletes existing forecasts if it has any and stores new values
        [HttpGet("forecast")]
        public async Task<IActionResult> GetForecast([FromQuery] double lat, [FromQuery] double lon)
        {
            var result = await _weatherService.GetForecastAsync(lat, lon);
            return Ok(result);
        }

        //Gets Forecast by Existing Location Id, deletes existing forecasts if it has any and stores new values
        [HttpGet("forecast/{id}")]
        public async Task<IActionResult> GetForecastByLocationId(int id)
        {
            var result = await _weatherService.GetForecastByIdAsync(id);
            return Ok(result);
        }

        //Creates a new Location record for given Coordinates.
        [HttpPost("locations")]
        public async Task<IActionResult> AddLocation([FromBody] Location location)
        {
            await _weatherService.AddLocationAsync(location.Latitude, location.Longitude);
            return NoContent();
        }

        //Gets all Locations info stored in Locations table
        [HttpGet("locations")]
        public async Task<IActionResult> GetAllLocations()
        {
            var locations = await _weatherService.GetLocationsAsync();
            return Ok(locations);
        }        

        //Deletes Location Record by Id. If Id does not exists, 404 Not Found is return
        [HttpDelete("locations/{id}")]
        public async Task<IActionResult> DeleteLocation(int id)
        {
            var location = await _weatherService.GetLocationByIdAsync(id);
            if (location == null)
                return NotFound();

            await _weatherService.DeleteLocationAsync(location);
            return NoContent();
        }
    }
}
