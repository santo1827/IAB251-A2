namespace interport.Models;

public class Quotation
{
    public int Id { get; set; }                                // Quotation number
    public int QuotationRequestId { get; set; }
    public QuotationRequest QuotationRequest { get; set; } = null!;
    public string ContainerType { get; set; } = null!;
    public string ScopeDescription { get; set; } = null!;
    public decimal ItemCharges { get; set; }
    public decimal DepotCharges { get; set; }
    public decimal LclDeliveryCharges { get; set; }
    public string Status { get; set; } = "Pending";            // Accept/Reject/Pending
    public DateTime DateIssuedUtc { get; set; } = DateTime.UtcNow;
}