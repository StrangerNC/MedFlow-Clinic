namespace UserManagementService.Dtos;

public class UserProfileReadDto
{
    public int Id { get; set; }
    public string Position { get; set; }
    public string Department { get; set; }
    public string FullName { get; set; }
    public DateTime EmploymentDate { get; set; }
}