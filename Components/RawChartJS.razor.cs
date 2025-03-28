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
