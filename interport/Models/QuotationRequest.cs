namespace interport.Models;

public class QuotationRequest
{
    public int Id { get; set; }                                // Request ID
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    public string Source { get; set; } = null!;
    public string Destination { get; set; } = null!;
    public int NumberOfContainers { get; set; }
    public string NatureOfPackage { get; set; } = null!;
    public string NatureOfJob { get; set; } = null!;           // import/export, pack/unpack, quarantine, etc.
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
}