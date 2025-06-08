using AuthService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;

namespace AuthService.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.Role).HasConversion(
                v => v.GetDisplayName(),
                v => Enum.Parse<Roles>(v));
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true);
            entity.Property(e => e.IsAdmin)
                .HasDefaultValue(false);
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("NOW()");
        });
    }
}