using Microsoft.JSInterop;

namespace StatusExposed.Services.Implementations;

public class ClipboardService : IClipboardService
{
    private readonly IJSRuntime jsRuntime;

    public ClipboardService(IJSRuntime jsRuntime)
    {
        this.jsRuntime = jsRuntime;
    }

    public async Task<string> ReadTextAsync()
    {
        return await jsRuntime.InvokeAsync<string>("navigator.clipboard.readText");
    }

    public async Task WriteTextAsync(string text)
    {
        await jsRuntime.InvokeVoidAsync("navigator.clipboard.writeText", text);
    }
}