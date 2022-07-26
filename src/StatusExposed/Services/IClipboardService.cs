namespace StatusExposed.Services;

public interface IClipboardService
{
    public Task<string> ReadTextAsync();

    public Task WriteTextAsync(string text);
}