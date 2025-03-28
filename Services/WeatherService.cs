using System.Net.Http.Json;
using BlazorWasmGuitarScales.Models;

namespace BlazorWasmGuitarScales.Services;

/// <summary>
/// Service for fetching weather data.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="WeatherService"/> class.
/// </remarks>
/// <param name="httpClient">The HTTP client to use for requests.</param>
public class WeatherService(HttpClient httpClient)
{
    /// <summary>
    /// Gets or sets the weather response.
    /// </summary>
    public WeatherResponse WeatherResponse { get; set; } = new WeatherResponse();

    /// <summary>
    /// Fetches weather data for a specified city and number of days ahead.
    /// </summary>
    /// <param name="city">The city to fetch weather data for.</param>
    /// <param name="daysAhead">The number of days ahead to fetch weather data for.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the weather response.</returns>
    /// <exception cref="Exception">Thrown when an error occurs while fetching weather data.</exception>
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
