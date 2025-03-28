# BlazorWasmGuitarScales

## Overview

BlazorWasmGuitarScales is a Blazor WebAssembly application designed to help users visualize guitar scales. The app leverages Blazor's component-based architecture to provide a rich, interactive user experience directly in the browser without the need for a server-side component.
Currently the app can is hosted on Azure and can be viewed at:


[**Guitar Scales App**](https://ambitious-plant-04398c103.6.azurestaticapps.net/)


## Progressive Web App (PWA)

This application is a Progressive Web App (PWA), which means it can be installed on your device and used offline. PWAs combine the best of web and mobile apps, providing a seamless experience across different devices. By installing the app, users can access it quickly from their home screen, receive push notifications, and enjoy faster load times.

### How to Install

1. Open the app in your browser.
2. Click on the install button in the address bar or the menu (depending on your browser).
3. Follow the prompts to add the app to your home screen.

## Functionality

### Weather Forecast

The app includes a weather forecast feature that displays a 5-day weather forecast for a specified city. This feature is implemented using the `WeatherService` class, which fetches weather data from an external API.

#### Components

- **WeatherTable.razor**: Displays the weather forecast in a table format.
- **WeatherChart.razor**: Visualizes the weather data using a line chart.

#### Services

- **WeatherService**: Fetches weather data from an external API and provides it to the components.

### Guitar Scales

The app provides information about various guitar scales, including major, minor, and pentatonic scales. Users can select a root note and a scale type to see the notes in that scale.

#### Components

- **ScaleService**: Provides methods to get available keys, scales, and the notes in a selected scale.

## How It Works

### Initialization

The app is initialized in the `Program.cs` file, where the Blazor WebAssembly host is configured, and services are registered.

    ```csharp
    var builder = WebAssemblyHostBuilder.CreateDefault(args);
    builder.RootComponents.Add<App>("#app");
    builder.RootComponents.Add<HeadOutlet>("head::after");

    builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
    builder.Services.AddScoped<WeatherService>();

    await builder.Build().RunAsync();
    ```

### Weather Data Fetching

The `WeatherService` class fetches weather data using the `HttpClient` service. It handles errors gracefully and provides the data to the components.


    ```csharp
    public class WeatherService
    {
        private readonly HttpClient httpClient;

        public WeatherService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<WeatherResponse?> GetWeatherDataAsync(string city, int daysAhead)
        {
            try
            {
                var response = await httpClient.GetFromJsonAsync<WeatherResponse>($"https://api.weatherapi.com/v1/forecast.json?key={Definer.Weather}&q={city}&days={daysAhead}&aqi=no&alerts=no");
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                throw new Exception("An unexpected error occurred. Please try again later.");
            }
        }
    }
    ```

### Guitar Scales

The `ScaleService` class provides methods to get the notes in a selected scale. It uses predefined patterns for different scale types and calculates the notes based on the root note and the scale pattern.


    ```csharp
    public static class ScaleService
    {
        public static NoteName[] Chromatic => Enum.GetValues<NoteName>();

        private static readonly Dictionary<ScaleType, List<ScaleStep>> ScalePatterns = new()
        {
            { ScaleType.Major, new() { ScaleStep.Whole, ScaleStep.Whole, ScaleStep.Half, ScaleStep.Whole, ScaleStep.Whole, ScaleStep.Whole, ScaleStep.Half } },
            { ScaleType.Minor, new() { ScaleStep.Whole, ScaleStep.Half, ScaleStep.Whole, ScaleStep.Whole, ScaleStep.Half, ScaleStep.Whole, ScaleStep.Whole } },
            { ScaleType.Pentatonic, new() { ScaleStep.Whole, ScaleStep.Whole, ScaleStep.WholeHalf, ScaleStep.Whole, ScaleStep.WholeHalf } }
        };

        public static List<NoteName> GetAvailableKeys() => Chromatic.ToList();
        public static List<ScaleType> GetAvailableScales() => ScalePatterns.Keys.ToList();

        public static NoteName GetNoteFrom(NoteName rootNote, int semitoneOffset)
        {
            int startIndex = (int)rootNote;
            return Chromatic[(startIndex + semitoneOffset) % 12];
        }

        public static HashSet<NoteName> GetScaleNotes(NoteName root, ScaleType scale)
        {
            if (!ScalePatterns.TryGetValue(scale, out var steps)) return new HashSet<NoteName>();

            var result = new HashSet<NoteName> { root };
            int semitoneOffset = 0;

            foreach (var step in steps)
            {
                semitoneOffset += step switch
                {
                    ScaleStep.Half => 1,
                    ScaleStep.Whole => 2,
                    ScaleStep.WholeHalf => 3,
                    _ => 0
                };

                result.Add(GetNoteFrom(root, semitoneOffset));
            }

            return result;
        }
    }
    ```

### Chart.js Integration

The app uses Chart.js to render charts for visualizing data. This is achieved through a combination of Blazor components and JavaScript interop.

#### Components

- **RawChartJS.razor**: A Blazor component that renders a Chart.js chart using a canvas element.


    ```html
    <canvas id="@Id"></canvas>

    ```

- **RawChartJS.razor.cs**: The code-behind for the `RawChartJS` component, which handles the JavaScript interop to set up the chart.


    ```csharp
    using Microsoft.AspNetCore.Components;
    using Microsoft.JSInterop;

    namespace BlazorWasmGuitarScales.Components;

    /// <summary>
    /// A Blazor component that renders a Chart.js chart using raw JavaScript interop.
    /// </summary>
    public partial class RawChartJS : ComponentBase
    {
        /// <summary>
        /// Gets or sets the JavaScript runtime.
        /// </summary>
        [Inject] public required IJSRuntime JSRuntime { get; set; }

        /// <summary>
        /// Gets or sets the ID of the chart element.
        /// </summary>
        [Parameter] public required string Id { get; set; }

        /// <summary>
        /// Gets or sets the configuration object for the chart.
        /// </summary>
        [Parameter] public required object Config { get; set; }

        /// <summary>
        /// Called after the component has been rendered.
        /// </summary>
        /// <param name="firstRender">Indicates if this is the first render.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await JSRuntime.InvokeVoidAsync("setupChart", Id, Config);
        }
    }

    ```

- **LineChartConfig.cs**: Provides the configuration object for the line chart.


    ```csharp
    namespace BlazorWasmGuitarScales.ChartConfig;

    /// <summary>
    /// Provides configuration for a line chart.
    /// </summary>
    public static class LineChartConfig
    {
        /// <summary>
        /// Gets the configuration object for the line chart.
        /// </summary>
        public static object Config => new
        {
            Type = "line",
            options = new
            {
                maintainAspectRatio = false,
            },
            Data = new
            {
                Datasets = new[]
                {
                    new {
                        label = "Data",
                        backgroundColor = "rgb(255, 99, 132)",
                        borderColor = "rgb(255, 99, 132)",
                        data = new [] { 0, 10, 5, 2, 20, 30, 45 },
                    }
                },
                Labels = new[]
                {
                    "January",
                    "February",
                    "March",
                    "April",
                    "May",
                    "June"
                }
            }
        };
    }

    ```

### JavaScript Interop

JavaScript Interop (JS Interop) in Blazor allows Blazor applications to call JavaScript functions and vice versa. This is particularly useful when needed to use existing JavaScript libraries or APIs that are not available in .NET.

#### Why Use JavaScript Interop in BlazorWasmGuitarScales

In the BlazorWasmGuitarScales application, JavaScript Interop is used to integrate Chart.js, a popular JavaScript library for creating charts. Blazor itself does not have built-in support for rendering charts, so leveraging Chart.js through JS Interop provides a powerful way to visualize data.

#### How JavaScript Interop is Implemented

1. **JavaScript Function**: The `rawchart.js` file contains the JavaScript function `setupChart` which initializes a Chart.js chart.

    
    ```javascript
        var charts = new Object();
        window.setupChart = (id, config) => {
            var ctx = document.getElementById(id).getContext('2d');
            if (typeof charts[id] !== 'undefined') { charts[id].destroy(); }
            charts[id] = new Chart(ctx, config);
        }
    
    ```

    - **`setupChart`**: This function takes an element ID and a configuration object to create or update a Chart.js chart.

2. **Blazor Component**: The `RawChartJS.razor` component defines a canvas element where the chart will be rendered.

    
    ```html
        <canvas id="@Id"></canvas>
    
    ```

3. **Component Code-Behind**: The `RawChartJS.razor.cs` file handles the JS Interop to call the `setupChart` function.

    
    ```csharp
        using Microsoft.AspNetCore.Components;
        using Microsoft.JSInterop;

        namespace BlazorWasmGuitarScales.Components;

        public partial class RawChartJS : ComponentBase
        {
            [Inject] public required IJSRuntime JSRuntime { get; set; }
            [Parameter] public required string Id { get; set; }
            [Parameter] public required object Config { get; set; }

            protected override async Task OnAfterRenderAsync(bool firstRender)
            {
                await JSRuntime.InvokeVoidAsync("setupChart", Id, Config);
            }
        }
    
    ```

    - **`JSRuntime`**: Injected to enable calling JavaScript functions.
    - **`OnAfterRenderAsync`**: Calls the `setupChart` function after the component is rendered.

#### Benefits of Using JavaScript Interop

- **Leverage Existing Libraries**: Utilize powerful JavaScript libraries like Chart.js without rewriting them in .NET.
- **Enhanced Functionality**: Add features to your Blazor app that are not natively supported.
- **Seamless Integration**: Combine the strengths of Blazor and JavaScript for a richer user experience.

By using JavaScript Interop, BlazorWasmGuitarScales can render dynamic and interactive charts, enhancing the application's ability to visualize guitar scales and weather data effectively.