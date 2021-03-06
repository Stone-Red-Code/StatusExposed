using StatusExposed.Models;

namespace StatusExposed.Services;

public interface IStatusService
{
    /// <summary>
    /// Gets the current status of a service.
    /// </summary>
    /// <param name="domain">Domain of the service.</param>
    /// <returns><see cref="StatusInformation"/> for the requested service.</returns>
    Task<StatusInformation?> GetStatusAsync(string domain);

    /// <summary>
    /// Adds a new service.
    /// </summary>
    /// <param name="domain">Domain of the service.</param>
    /// <param name="statusPageUrl">Official status page of the service.</param>
    /// <returns>A <see cref="Task"/> to await</returns>
    Task AddServiceAsync(string domain, string? statusPageUrl);

    /// <summary>
    /// Gets the statuses of all services in a specified range.
    /// </summary>
    /// <param name="index">Start index of services.</param>
    /// <param name="count">The max amount of services to return.</param>
    /// <returns>The <see cref="StatusInformation"/>s of all services.</returns>
    IEnumerable<StatusInformation> GetStatuses(int index, int count);
}