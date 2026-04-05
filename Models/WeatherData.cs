namespace Katalog.Models
{
    public class WeatherResponse
    {
        public WeatherMain Main { get; set; } = new();
        public List<WeatherDescription> Weather { get; set; } = new();
        public string Name { get; set; } = string.Empty;
    }
    public class WeatherMain
    {
        public double Temp { get; set; }
    }

    public class WeatherDescription
    {
        public string Main { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}