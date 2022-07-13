using System.ComponentModel.DataAnnotations;

namespace StatusExposed.Models;

public class StatusInformation
{
    [Key]
    public string ServicePageDomain { get; init; } = string.Empty;

    public string? StatusPageUrl { get; set; }
    public List<StatusHistoryData> StatusHistory { get; set; } = new List<StatusHistoryData>();

    public StatusHistoryData CurrentStatusHistoryData => StatusHistory.OrderByDescending(s => s.LastUpdateTime).FirstOrDefault() ?? new StatusHistoryData();
}