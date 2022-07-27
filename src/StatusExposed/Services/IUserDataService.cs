using StatusExposed.Models;

namespace StatusExposed.Services;

public interface IUserDataService
{
    /// <summary>
    /// Subscribes the a service.
    /// </summary>
    /// <param name="domain">The domain of the service.</param>
    /// <returns>A <see cref="Task"/> to <see langword="await"/> and a result message.</returns>
    Task<(bool Success, string? ErrorMessage)> SubscribeToServiceAsync(string domain);

    /// <summary>
    /// Unsubscribes from a service
    /// </summary>
    /// <param name="domain">The domain of the service.</param>
    /// <returns>A <see cref="Task"/> to <see langword="await"/> and a result message.</returns>
    Task<(bool Success, string? ErrorMessage)> UnsubscribeFromServiceAsync(string domain);

    /// <summary>
    /// Gets all services the user has subscribed to.
    /// </summary>
    /// <returns>A <see cref="Task"/> to <see langword="await"/> and all services the user has subscribed to.</returns>
    Task<IEnumerable<ServiceInformation>?> GetAllSubscribedServicesAsync();

    /// <summary>
    /// Gets the max amount of services the user can subscribe to.
    /// </summary>
    /// <returns>A <see cref="Task"/> to <see langword="await"/> and the max amount of services a user can subscribe to.</returns>
    Task<int> GetSiteSubscribtionsLimitAsync();

    /// <summary>
    /// Generates a new API key
    /// </summary>
    /// <returns>A <see cref="Task"/> to <see langword="await"/>.</returns>
    Task GenerateNewApiKeyAsync();

    /// <summary>
    /// Remove an API key
    /// </summary>
    /// <param name="apiKey">The APi key to be removed.</param>
    /// <returns>A <see cref="Task"/> to <see langword="await"/>.</returns>
    Task RemoveApiKeyAsync(ApiKey apiKey);

    /// <summary>
    /// Gets all API key of the user.
    /// </summary>
    /// <returns>A <see cref="Task"/> to <see langword="await"/> and a <see cref="List{T}"/> of all API keys of the user.</returns>
    Task<List<ApiKey>?> GetApiKeysAsync();

    /// <summary>
    /// Gets the max amount of API keys the user can have.
    /// </summary>
    /// <returns>A <see cref="Task"/> to <see langword="await"/> and the max amount of API keys a user can subscribe to.</returns>
    Task<int> GetSiteApiKeysLimitAsync();
}