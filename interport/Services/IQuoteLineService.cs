using interport.Models;

namespace interport.Services

{
    // Handles quote lines logic.
    public interface IQuoteLineService
    {
        Task<Quotation> AddLineAsync(int quotationId, string description, int quantity, decimal unitPrice);
        Task<Quotation> RemoveLineAsync(int quotationId, int lineId);
        Task<Quotation> RecalculateAsync(int quotationId);
        
        
        
        
        //Add Fees to a quote.
        Task<Quotation> AddFeeAsync(int quotationId, Enums.FeeType fee, int quantity = 1);
        Task<Quotation> AddFeesAsync(int quotationId, IEnumerable<Enums.FeeType> fees, int feeQuantity = 1);
        
    }
}
