using System.ComponentModel.DataAnnotations;

namespace MedicalRecordService.Dtos;

public class AppointmentPublishedDto
{
    public int ExternalId { get; set; }
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public string Status { get; set; }
}