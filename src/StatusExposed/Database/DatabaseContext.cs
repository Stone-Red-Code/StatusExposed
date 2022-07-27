using Microsoft.EntityFrameworkCore;

using StatusExposed.Models;

namespace StatusExposed.Database;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions<DatabaseContext> dbContextOptions) : base(dbContextOptions)
    {
    }

    public DbSet<ServiceInformation> Services { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Subscriber> Subscriber { get; set; } = null!;
}