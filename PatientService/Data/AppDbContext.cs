using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;
using PatientService.Models;

namespace PatientService.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Patient> Patients { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Patient>(entity =>
        {
            entity.Property(e => e.Gender)
                .HasConversion(
                    v => v.GetDisplayName(),
                    v => Enum.Parse<Genders>(v));
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("NOW()");
            entity.Property(e => e.IsTransferred)
                .HasDefaultValue(false);
        });
    }
}