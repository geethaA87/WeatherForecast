using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherForecast.Domain.Models
{
    public class Location
    {
        public int Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public ICollection<WeatherForecastRecord> Forecasts { get; set; } = new List<WeatherForecastRecord>();

    }
}
