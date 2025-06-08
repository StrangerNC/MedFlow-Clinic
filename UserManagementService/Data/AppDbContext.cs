using Microsoft.EntityFrameworkCore;
using UserManagementService.Models;

namespace UserManagementService.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.Property(x => x.IsTransferred)
                .HasDefaultValue(false);
        });
        modelBuilder.Entity<UserProfile>(entity =>
            {
                entity.HasOne(x => x.User)
                    .WithMany(x => x.UserProfiles)
                    .HasForeignKey(x => x.UserId);
            }
        );
    }
}