using System.Net.Http.Json;
using BlazorWasmGuitarScales.Models;

namespace BlazorWasmGuitarScales.Services;

public class WeatherService(HttpClient httpClient)
{
    public WeatherResponse WeatherResponse { get; set; } = new WeatherResponse();

    public async Task<WeatherResponse?> GetWeatherDataAsync(string city, int daysAhead)
    {
        try
        {
            var response = await httpClient.GetFromJsonAsync<WeatherResponse>($"https://api.weatherapi.com/v1/forecast.json?key={Definer.Weather}&q={city}&days={daysAhead}&aqi=no&alerts=no") ?? throw new Exception("No data received from the weather API.");
            return response;
        }
        catch (HttpRequestException httpRequestException)
        {
            // Handle HTTP request errors (e.g., network issues)
            Console.WriteLine($"Request error: {httpRequestException.Message}");
            throw new Exception("Error fetching weather data. Please try again later.");
        }
        catch (Exception ex)
        {
            // Handle other errors
            Console.WriteLine($"An error occurred: {ex.Message}");
            throw new Exception("An unexpected error occurred. Please try again later.");
        }
    }
}
