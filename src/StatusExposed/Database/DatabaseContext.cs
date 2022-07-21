using Microsoft.EntityFrameworkCore;

using StatusExposed.Models;

namespace StatusExposed.Database;

public class DatabaseContext : DbContext
{
    public DbSet<ServiceInformation> Services { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;

    public DbSet<Subscriber> Subscriber { get; set; } = null!;

    public string DbPath { get; }

    public DatabaseContext(IDatabaseConfiguration configuration)
    {
        DbPath = configuration.Name;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        _ = optionsBuilder.UseSqlite($"Data Source={DbPath}");
    }
}