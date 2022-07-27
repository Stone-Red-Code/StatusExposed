namespace StatusExposed.Services;

public interface IClipboardService
{
    /// <summary>
    /// Reads text from the clipboard.
    /// </summary>
    /// <returns>A <see cref="Task"/> to <see langword="await"/> and the <see cref="string"/> with the text of the clipboard.</returns>
    public Task<string> ReadTextAsync();

    /// <summary>
    /// Writes text to the clipboard.
    /// </summary>
    /// <param name="text">The text to be written to the clipboard.</param>
    /// <returns>A <see cref="Task"/> to <see langword="await"/>.</returns>
    public Task WriteTextAsync(string text);
}