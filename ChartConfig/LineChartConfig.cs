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
