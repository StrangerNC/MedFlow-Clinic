using System.ComponentModel.DataAnnotations;

namespace TransferService.Models;

public class ClinicData
{
    [Key] [Required] public int Id { get; set; }
    [Required] public string Name { get; set; }
    [Required] public string Address { get; set; }
    [Required] public string ContactPerson { get; set; }
    [Required] public string ContactEmail { get; set; }
    [Required] public string Region { get; set; }
    [Required] public string MinzdravIPAddress { get; set; }
}