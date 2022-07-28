using StatusExposed.Utilities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StatusExposed.Models;

public class StatusData
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int Id { get; set; }

    public Status Status { get; set; } = Status.Unknown;
    public DateTime LastUpdateTime { get; set; }
    public TimeSpan ResponseTime { get; set; } = TimeSpan.MaxValue;

    public string FormattedLastUpdateTime => (DateTime.UtcNow - LastUpdateTime).ToRelevantTimeUnitString();
    public string FormattedResponseTimeTime => ResponseTime.ToRelevantTimeUnitString();
}