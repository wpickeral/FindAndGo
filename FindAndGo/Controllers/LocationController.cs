namespace FindAndGo.Controllers;

public class LocationController
{
    var apiKey = Environment.GetEnvironmentVariable("WEATHER_API_KEY");
    var weather = new OpenWeatherMapAPI(new HttpClient(), apiKey);
}