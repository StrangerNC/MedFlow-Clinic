namespace AppointmentService.Dtos;

public class AppointmentPublishDto
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public string Status { get; set; }
    public string Reason { get; set; }
}