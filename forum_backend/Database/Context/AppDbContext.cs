using forum_backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace forum_backend.Database.Context;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Threads>()
            .HasOne(t => t.SupThread)
            .WithMany()
            .HasForeignKey(t => t.SupThreadId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Threads>()
            .HasOne(t => t.PrimeThread)
            .WithMany()
            .HasForeignKey(t => t.PrimeThreadId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Threads>()
            .HasMany(t => t.ThreadImages)
            .WithOne(ti => ti.Thread)
            .HasForeignKey(ti => ti.ThreadId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Bans>()
            .HasOne(b => b.BannedUser)
            .WithMany()
            .HasForeignKey(b => b.BannedUserId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Bans>()
            .HasOne(b => b.BanningModerator)
            .WithMany()
            .HasForeignKey(b => b.BanningModeratorId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Reports>()
            .HasOne(r => r.ReportingUser)
            .WithMany()
            .HasForeignKey(r => r.ReportingUserId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Reports>()
            .HasOne(r => r.ReportedUser)
            .WithMany()
            .HasForeignKey(r => r.ReportedUserId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Likes>()
            .HasOne(l => l.User)
            .WithMany()
            .HasForeignKey(l => l.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Likes>()
            .HasOne(l => l.Thread)
            .WithMany()
            .HasForeignKey(l => l.ThreadId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Subscriptions>()
            .HasOne(s => s.User)
            .WithMany()
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Subscriptions>()
            .HasOne(s => s.Thread)
            .WithMany()
            .HasForeignKey(s => s.ThreadId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<ThreadTags>().HasKey(tt => new { tt.ThreadId, tt.TagId });

        base.OnModelCreating(modelBuilder);
    }

    public DbSet<Bans> Bans { get; set; }
    public DbSet<Images> Images { get; set; }
    public DbSet<Likes> Likes { get; set; }
    public DbSet<Reports> Reports { get; set; }
    public DbSet<Subscriptions> Subscriptions { get; set; }
    public DbSet<Tags> Tags { get; set; }
    public DbSet<Threads> Threads { get; set; }
    public DbSet<ThreadTags> ThreadTags { get; set; }
    public DbSet<Users> Users { get; set; }
}