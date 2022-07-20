using StatusExposed.Models;

namespace StatusExposed.Services;

public interface IUserDataService
{
    Task<(bool Success, string? ErrorMessage)> SubscribeToServiceAsync(string domain);

    Task<(bool Success, string? ErrorMessage)> UnsubscribeFromServiceAsync(string domain);

    Task<IEnumerable<ServiceInformation>?> GetAllSubscribedServicesAsync();
}