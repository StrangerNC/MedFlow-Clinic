using AppointmentService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;

namespace AppointmentService.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Appointment> Appointments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.Property(x => x.Status)
                .HasConversion(
                    v => v.GetDisplayName(),
                    v => Enum.Parse<Statuses>(v));
            entity.Property(x => x.IsTransferred)
                .HasDefaultValue(false);
            entity.Property(x => x.CreatedAt)
                .HasDefaultValueSql("NOW()");
        });
    }
}