namespace StatusExposed.Database;

public class DatabaseConfiguration : IDatabaseConfiguration
{
    public string Name { get; }

    public DatabaseConfiguration(string name)
    {
        Name = name;
    }
}