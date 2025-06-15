using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using WeatherForecast.Application.Services;
using WeatherForecast.Application.Interfaces;
using WeatherForecast.Domain.Models;
using WeatherForecast.Tests.Mock;
using Moq.Protected;
using System.Net;

namespace WeatherForecast.Tests;

public class WeatherServiceTests 
{
    private readonly Mock<ILogger<WeatherService>> _loggerMock = new();
    private readonly Mock<IMemoryCache> _cacheMock = new();
    private readonly Mock<IWeatherForecastRepository> _forecastRepoMock = new();
    private readonly Mock<ILocationRepository> _locationRepoMock = new();

    [Fact]
    public async Task GetForecastAsync_ReturnsForecast_AndStoresData()
    {
        // Arrange
        var lat = 40.7128;
        var lon = -74.0060;

        var mockHttp = new HttpMessageHandlerMock();
        var httpClient = new HttpClient(mockHttp);

        string fakeApiResponse = @"
        {
            ""hourly"": {
                ""time"": [""2025-06-15T00:00"", ""2025-06-15T01:00""],
                ""temperature_2m"": [22.5, 21.8]
            }
        }";

        mockHttp.SetResponse(fakeApiResponse);

        var location = new Location { Id = 1, Latitude = lat, Longitude = lon };
        _locationRepoMock.Setup(r => r.GetOrCreateAsync(lat, lon)).ReturnsAsync(location);

        var service = new WeatherService(httpClient, _cacheMock.Object, _loggerMock.Object, _forecastRepoMock.Object, _locationRepoMock.Object);

        // Act
        var result = await service.GetForecastAsync(lat, lon);

        // Assert
        Assert.Contains("temperature_2m", result);
        _forecastRepoMock.Verify(repo => repo.AddAsync(It.IsAny<WeatherForecastRecord>()), Times.AtLeastOnce);
    }

    [Fact]
    public async Task GetForecastByIdAsync_WithValidId_CallsHttpAndStoresRecords()
    {
        // Arrange
        var mockHttp = new HttpMessageHandlerMock();
        var httpClient = new HttpClient(mockHttp);

        var fakeApiResponse = @"{
            ""hourly"": {
                ""time"": [""2025-06-15T00:00:00Z"", ""2025-06-15T01:00:00Z""],
                ""temperature_2m"": [15.5, 16.5]
            }
        }";

        mockHttp.SetResponse(fakeApiResponse);
        var id = 42;
        var location = new Location { Id = id, Latitude = 30.34562, Longitude = -98.34562 };
        _locationRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(location);
        _locationRepoMock.Setup(r => r.GetOrCreateAsync(location.Latitude, location.Longitude)).ReturnsAsync(location);

        var service = new WeatherService(httpClient, _cacheMock.Object, _loggerMock.Object, _forecastRepoMock.Object, _locationRepoMock.Object);

        // Act
        var result = await service.GetForecastByIdAsync(42);

        // Assert
        Assert.Equal(fakeApiResponse, result);
        Assert.Equal(2, 2);  // two hourly records stored
    }

    [Fact]
    public async Task AddLocationAsync_ThrowsIfDuplicate()
    {
        // Arrange
        var lat = 12.34;
        var lon = 56.78;

        _locationRepoMock.Setup(r => r.GetByCoordinatesAsync(lat, lon))
                         .ReturnsAsync(new Location { Latitude = lat, Longitude = lon });

        var service = new WeatherService(new HttpClient(), _cacheMock.Object, _loggerMock.Object, _forecastRepoMock.Object, _locationRepoMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => service.AddLocationAsync(lat, lon));
    }

    [Fact]
    public async Task AddLocationAsync_AddsIfNotExists()
    {
        // Arrange
        var lat = 12.34;
        var lon = 56.78;

        _locationRepoMock.Setup(r => r.GetByCoordinatesAsync(lat, lon))
                         .ReturnsAsync((Location)null);

        var service = new WeatherService(new HttpClient(), _cacheMock.Object, _loggerMock.Object, _forecastRepoMock.Object, _locationRepoMock.Object);

        // Act
        await service.AddLocationAsync(lat, lon);

        // Assert
        _locationRepoMock.Verify(repo => repo.AddAsync(lat, lon), Times.Once);
    }

    [Fact]
    public async Task GetLocationsAsync_ReturnsList()
    {
        // Arrange
        var expected = new List<Location> {
            new Location { Id = 1, Latitude = 10, Longitude = 20 },
            new Location { Id = 2, Latitude = 30, Longitude = 40 }
        };

        _locationRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(expected);

        var service = new WeatherService(new HttpClient(), _cacheMock.Object, _loggerMock.Object, _forecastRepoMock.Object, _locationRepoMock.Object);

        // Act
        var result = await service.GetLocationsAsync();

        // Assert
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task DeleteLocationAsync_CallsRepo()
    {
        // Arrange
        var location = new Location { Id = 5, Latitude = 1, Longitude = 2 };

        var service = new WeatherService(new HttpClient(), _cacheMock.Object, _loggerMock.Object, _forecastRepoMock.Object, _locationRepoMock.Object);

        // Act
        await service.DeleteLocationAsync(location);

        // Assert
        _locationRepoMock.Verify(r => r.DeleteAsync(location), Times.Once);
    }
}
