using Microsoft.EntityFrameworkCore;
using TransferService.Models;

namespace TransferService.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<MedicalRecord> MedicalRecords { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<ClinicData> ClinicData { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<Visit> Visits { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MedicalRecord>(entity => { entity.Property(x => x.IsSent).HasDefaultValue(false); });
        modelBuilder.Entity<Appointment>(entity => { entity.Property(x => x.IsSent).HasDefaultValue(false); });
        modelBuilder.Entity<Patient>(entity => { entity.Property(x => x.IsSent).HasDefaultValue(false); });
        modelBuilder.Entity<UserProfile>(entity => { entity.Property(x => x.IsSent).HasDefaultValue(false); });
        modelBuilder.Entity<Visit>(entity => { entity.Property(x => x.IsSent).HasDefaultValue(false); });
    }
}