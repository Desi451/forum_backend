using forum_backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace forum_backend.Database.Context;

public class AppDbContext : DbContext
{
    public AppDbContext() { }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Tag> Tags { get; set; }
}
