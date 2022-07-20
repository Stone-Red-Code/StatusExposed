using StatusExposed.Models;

namespace StatusExposed.Services;

public interface IStatusService
{
    /// <summary>
    /// Gets the current status of a service.
    /// </summary>
    /// <param name="domain">Domain of the service.</param>
    /// <returns><see cref="ServiceInformation"/> for the requested service or <see langword="null"/> if not found.</returns>
    Task<ServiceInformation?> GetStatusAsync(string domain);

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
    /// <returns>The <see cref="ServiceInformation"/>s of all services.</returns>
    IEnumerable<ServiceInformation> GetStatuses(int index, int count);

    /// <summary>
    /// Updates the status of the specified domain.
    /// </summary>
    /// <param name="domain">Domain of the service.</param>
    /// <returns>A <see cref="Task"/> to await</returns>
    Task UpdateStatusAsync(string domain);
}