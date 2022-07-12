using StatusExposed.Utilities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StatusExposed.Models;

public class StatusHistoryData
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int Id { get; set; }

    public DateTime LastUpdateTime { get; set; }

    public Status Status { get; set; }
    public TimeSpan Ping { get; set; }

    public string FormatedUpdateTime => (DateTime.UtcNow - LastUpdateTime).ToRelevantTimeUnitString();
    public string FormatedPingTime => Ping.ToRelevantTimeUnitString();
}