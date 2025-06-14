namespace MedicalRecordService.Dtos;

public class VisitReadDto
{
    public int Id { get; set; }
    public int MedicalRecordId { get; set; }
    public int AppointmentId { get; set; }
    public DateTime VisitDate { get; set; }
    public string ChiefComplaint { get; set; }
    public string Diagnosis { get; set; }
    public string TreatmentPlan { get; set; }
    public string Notes { get; set; }
}