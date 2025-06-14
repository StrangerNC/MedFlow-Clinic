namespace MedicalRecordService.Dtos;

public class MedicalRecordPublishDto
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}