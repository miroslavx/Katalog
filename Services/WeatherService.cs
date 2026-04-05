using System.Net.Http.Json;
using Katalog.Models;

namespace Katalog.Services
{
    public class WeatherService
    {
        private readonly HttpClient _httpClient;

     
        private const string ApiKey = "";
        private const string City = "Tallinn";

        public WeatherService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<WeatherResponse?> GetWeatherAsync()
        {
            try
            {
                string url = $"https://api.openweathermap.org/data/2.5/weather?q={City}&appid={ApiKey}&units=metric";

                var response = await _httpClient.GetFromJsonAsync<WeatherResponse>(url);
                return response;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}