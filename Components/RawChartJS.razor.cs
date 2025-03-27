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
