using Microsoft.EntityFrameworkCore;
using UserDirectory.Api.Models;

namespace UserDirectory.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<LocalAccount> LocalAccounts => Set<LocalAccount>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>().HasKey(u => u.Id);
        modelBuilder.Entity<LocalAccount>(account =>
        {
            account.HasKey(a => a.Id);
            account.HasIndex(a => a.NormalizedEmail).IsUnique();
            account.Property(a => a.Email).HasMaxLength(254).IsRequired();
            account.Property(a => a.NormalizedEmail).HasMaxLength(254).IsRequired();
            account.Property(a => a.PasswordHash).IsRequired();
        });
    }
}
