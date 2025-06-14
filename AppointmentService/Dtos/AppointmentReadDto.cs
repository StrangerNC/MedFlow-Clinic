using AppointmentService.Models;

namespace AppointmentService.Dtos;

public class AppointmentReadDto
{
    public int Id { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string Status { get; set; }
    public string Reason { get; set; }
    public DateTime UpdateAt { get; set; }
}