using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace StatusExposed.Models;

public class ServiceInformation
{
    [Key]
    public string ServicePageDomain { get; init; } = string.Empty;

    public string? StatusPageUrl { get; set; }
    public List<StatusData> StatusHistory { get; set; } = new List<StatusData>();

    [JsonIgnore]
    public List<Subscriber> Subscribers { get; set; } = new List<Subscriber>();

    public StatusData CurrentStatus => StatusHistory.OrderByDescending(s => s.LastUpdateTime).FirstOrDefault() ?? new StatusData();
}