namespace StatusExposed.Services;

public interface IScheduledUpdateService
{
    void Start(TimeSpan updateRate);

    void Stop();
}