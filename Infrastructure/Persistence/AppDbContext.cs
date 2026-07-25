using Microsoft.EntityFrameworkCore;
using Shortly.Domain.Entities;

namespace Shortly.Infrastructure.Persistence;

/// <summary>
/// Represents the application's database context, which is responsible for managing the connection to the database and
/// providing access to the entities (User and Link) through DbSet properties.
/// </summary>
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; private set; } = null!;

    public DbSet<Link> Links { get; private set; } = null!;
}
