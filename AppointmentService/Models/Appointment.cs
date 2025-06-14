using System.ComponentModel.DataAnnotations;

namespace AppointmentService.Models;

public class Appointment
{
    [Key] [Required] public int Id { get; set; }
    [Required] public int PatientId { get; set; }
    [Required] public int DoctorId { get; set; }
    [Required] public DateTime AppointmentDate { get; set; }
    [Required] public Statuses Status { get; set; }
    [Required] public string Reason { get; set; }
    [Required] public DateTime CreatedAt { get; set; }
    [Required] public DateTime UpdatedAt { get; set; }
    [Required] public bool IsTransferred { get; set; }
    public Patient Patient { get; set; }
    public Doctor Doctor { get; set; }
}

public enum Statuses
{
    Sheduled,
    Completed,
    Cancelled,
    Resheduled
}