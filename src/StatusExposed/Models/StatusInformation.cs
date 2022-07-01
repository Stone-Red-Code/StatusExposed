using StatusExposed.Utilities;

using System.ComponentModel.DataAnnotations;

namespace StatusExposed.Models;

public class StatusInformation
{
    [Key]
    public string ServicePageDomain { get; init; } = string.Empty;

    public string? StatusPageUrl { get; set; }
    public Status Status { get; set; }
    public DateTime LastUpdateTime { get; set; }
    public TimeSpan Ping { get; set; }
    public string FormatedUpdateTime => (DateTime.UtcNow - LastUpdateTime).ToRelevantTimeUnitString();
    public string FormatedPingTime => Ping.ToRelevantTimeUnitString();
}