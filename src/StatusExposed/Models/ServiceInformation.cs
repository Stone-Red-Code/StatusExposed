using System.ComponentModel.DataAnnotations;

namespace StatusExposed.Models;

public class ServiceInformation
{
    [Key]
    public string ServicePageDomain { get; init; } = string.Empty;

    public string? StatusPageUrl { get; set; }
    public List<StatusData> StatusHistory { get; set; } = new List<StatusData>();
    public List<Subscriber> Subscribers { get; set; } = new List<Subscriber>();

    public StatusData CurrentStatus => StatusHistory.OrderByDescending(s => s.LastUpdateTime).FirstOrDefault() ?? new StatusData();
}