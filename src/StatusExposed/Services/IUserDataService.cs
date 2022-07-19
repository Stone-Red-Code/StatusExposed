using StatusExposed.Models;

namespace StatusExposed.Services;

public interface IUserDataService
{
    Task<bool> SubscribeToServiceAsync(string domain);

    Task<bool> UnsubscribeFromServiceAsync(string domain);

    Task<IEnumerable<ServiceInformation>?> GetAllSubscribedServicesAsync();
}