using StatusExposed.Models;

namespace StatusExposed.Services;

public interface IStatusService
{
    Task<StatusInformation?> GetStatusAsync(string domain);

    Task AddServiceAsync(string domain, string? statusPageUrl);

    IEnumerable<StatusInformation> GetStatuses(int index, int count);
}