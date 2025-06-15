using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WeatherForecast.Tests.Mock
{
    public class HttpMessageHandlerMock : HttpMessageHandler
    {
        private string _mockResponse;

        public void SetResponse(string response)
        {
            _mockResponse = response;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var message = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(_mockResponse, Encoding.UTF8, "application/json")
            };
            return Task.FromResult(message);
        }
    }
}
