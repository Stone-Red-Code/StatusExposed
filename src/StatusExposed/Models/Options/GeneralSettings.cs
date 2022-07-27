using StatusExposed.Utilities;

namespace StatusExposed.Models.Options;

public class GeneralSettings
{
    public string UpdatePeriod { get; set; } = "10m";
    public TimeSpan UpdatePeriodTimeSpan => UpdatePeriod.ToTimeSpan();
    public bool AutomaticUpdates { get; set; }
    public InfoFilePaths FilePaths { get; set; } = new InfoFilePaths();
}