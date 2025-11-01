using System.ComponentModel.DataAnnotations;
namespace interport.Models;

public class QuotationRequest
{
    public int Id { get; set; }                                // request ID
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    
    [Required] public string Source { get; set; } = null!;
    [Required] public string Destination { get; set; } = null!;
    [Required] [Range(0, 999)] public int NumberOfContainers { get; set; }
    [Required] public string NatureOfPackage { get; set; } = null!;
    [Required] public string NatureOfJob { get; set; } = null!; 
    [Required] public Enums.ContainerType ContainerType { get; set; }
    
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

    public Enums.RequestStatus Status { get; set; } = Enums.RequestStatus.Pending;  // Default is pending
    public string? OfficerComments { get; set; }
    


}