using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherForecast.Domain.Models
{
    public class WeatherForecastRecord
    {
        public int Id { get; set; }
        public int LocationId { get; set; }
        public Location Location { get; set; } = null!;

        public DateTime Timestamp { get; set; }
        public double Temperature { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;

    }
}
