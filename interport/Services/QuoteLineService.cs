using interport.Data;
using interport.Models;
using Microsoft.EntityFrameworkCore;

namespace interport.Services;

public class QuoteLineService : IQuoteLineService
{
    private readonly AppDbContext _database;
    public QuoteLineService(AppDbContext db) => _database = db;
    
    
    // Async add a line to a quote and recalculate totals.
    public async Task<Quotation> AddLineAsync(int quotationId, string description, int quantity, decimal unitPrice)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description is required.");
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be >= 1.");
        if (unitPrice < 0)
            throw new ArgumentOutOfRangeException(nameof(unitPrice), "Unit price cannot be negative.");
        var quote = await LoadQuoteAsync(quotationId);

        
        var line = new QuoteLine
        {
            Description = description.Trim(),
            Quantity = quantity,
            UnitPrice = unitPrice,
            LineTotal = quantity * unitPrice //Calculate line total
        };

        quote.Lines.Add(line);
        RecalculateTotals(quote);

        await _database.SaveChangesAsync();
        return quote;
    }
    
    
    //remove a line from a quote and recalculate totals. Return task (async quote)
    public async Task<Quotation> RemoveLineAsync(int quotationId, int lineId)
    {
        var quote = await _database.Quotations
                        .Include(quote => quote.Lines)
                        .FirstOrDefaultAsync(quote => quote.Id == quotationId)
                    ?? throw new InvalidOperationException("quotation not found");

        var line = quote.Lines.FirstOrDefault(quoteLine => quoteLine.Id == lineId);
        if (line != null)
        {
            _database.QuotationLines.Remove(line);
            RecalculateTotals(quote);
            await _database.SaveChangesAsync();
        }

        return quote;
    }
    
    //Async call to recalculate totals using helper.
    public async Task<Quotation> RecalculateAsync(int quotationId)
    {
        var quote = await LoadQuoteAsync(quotationId);
        RecalculateTotals(quote);
        await _database.SaveChangesAsync();
        return quote;
    }
    
    
    
    
    
    
    
    //Helper methods for async quote functions.
    private async Task<Quotation> LoadQuoteAsync(int quotationId)
    {
        var quotation = await _database.Quotations
            .Include(quote => quote.Lines) //Query related lines
            .Include(quote => quote.QuotationRequest) //also fetch the associated request
            .FirstOrDefaultAsync(quote => quote.Id == quotationId); //Find the quote by its  ID

        return quotation ?? throw new InvalidOperationException("quote not found."); // throw error if quote isnt found.
    }
    
    
    //recalculate totals for quote pricing after changes.
    private static void RecalculateTotals(Quotation quote)
    {
        var subtotal = quote.Lines.Sum(quoteLine => quoteLine.LineTotal);
        quote.Subtotal = subtotal;
        quote.Discount = 0m;
        quote.Total = subtotal;
    }
    
    
}