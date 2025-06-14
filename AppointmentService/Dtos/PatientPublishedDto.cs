namespace AppointmentService.Dtos;

public class PatientPublishedDto
{
    public int ExternalId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? MiddleName { get; set; }
    public string PhoneNumber { get; set; }
}