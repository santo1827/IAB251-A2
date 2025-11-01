using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace interport.Models;

public class Quotation
{
    [Key] public int Id { get; set; } // Quotation number - logic only.
    
    
    public int QuotationRequestId { get; set; } //DB fk
    public QuotationRequest QuotationRequest { get; set; } = null!; //the associated request
    [Required] public string QuoteNumber { get; set; } = ""; // Displayed quotation ID
    
    
    public Enums.ContainerType ContainerType{ get; set; }
    public string ScopeDescription { get; set; } = null!; // Description / scope of the job
    
    public decimal ItemCharges { get; set; }
    public decimal DepotCharges { get; set; }
    public decimal LclDeliveryCharges { get; set; }
    public string Status { get; set; } = "Pending"; // Accept/Reject/Pending
    public DateTime DateIssuedUtc { get; set; } = DateTime.UtcNow; 
    
    
    
    //Pricing - Maximum is 999 Billion with 0.0001 precision in DB.
    [Column(TypeName="decimal(16,4)")] public decimal Subtotal { get; set; }
    [Column(TypeName="decimal(16,4)")] public decimal Discount { get; set; }
    [Column(TypeName="decimal(16,4)")] public decimal Total { get; set; }
    
    public List<QuoteLine> Lines { get; set; } = [];
}

public class QuoteLine
{
    //Line details
    public int Id { get; set; } //Item 
    public int QuotationId { get; set; }
    public Quotation? Quotation { get; set; }
    
    //Item details
    [Required] public string Description { get; set; } = "";
    public int Quantity { get; set; } = 1;
    
    [Column(TypeName="decimal(16,4)")] public decimal UnitPrice { get; set; }
    [Column(TypeName="decimal(16,4)")] public decimal LineTotal { get; set; }
    
}