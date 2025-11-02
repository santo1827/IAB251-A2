using interport.Data;
using interport.Models;
using Microsoft.EntityFrameworkCore;

namespace interport.Services;

public class QuoteLineService : IQuoteLineService
{
    private readonly AppDbContext _database;

    private readonly IRateScheduleService _rates;

    public QuoteLineService(AppDbContext db, IRateScheduleService rates)
    {
        _database = db;
        _rates = rates;
    }


    // Async add a line to a quote and recalculate totals.
    public async Task<Quotation> AddLineAsync(int quotationId, string description, int quantity, decimal unitPrice)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description is required.");
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity must be morre than or equal to 1.");
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
        
        //find line based on lineId
        var line = quote.Lines.FirstOrDefault(quoteLine => quoteLine.Id == lineId);
        //remove the line if found.
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
    
    
    //Add an individual fee to a quote.
    public async Task<Quotation> AddFeeAsync(int quotationId, Enums.FeeType fee, int quantity = 1)
    {
        //Load the quote.
        var quote = await LoadQuoteAsync(quotationId);

        //Get fee pricing and the ui friendly label.
        var unit = _rates.GetUnitPrice(quote.ContainerType, fee);
        var desc = _rates.GetLabel(fee);

        // Add the line to the quote.
        quote.Lines.Add(new QuoteLine
        {
            Description = desc,
            Quantity = Math.Max(1, quantity),
            UnitPrice = unit,
            LineTotal = Math.Max(1, quantity) * unit
        });

        //recalculate subtotal, total and gst
        RecalculateTotals(quote);
        await _database.SaveChangesAsync();
        return quote;
    }
    
    
    
    //Add many fees to a quote method.,
    public async Task<Quotation> AddFeesAsync(int quotationId, IEnumerable<Enums.FeeType> fees, int quantityPerFee = 1)
    {
        //Load the quote
        var quote = await LoadQuoteAsync(quotationId);

        //add each fee to the quote with its title, quantity and total.
        foreach (var fee in fees.Distinct())
        {
            var unit = _rates.GetUnitPrice(quote.ContainerType, fee);
            var desc = _rates.GetLabel(fee);

            quote.Lines.Add(new QuoteLine
            {
                Description = desc,
                Quantity = Math.Max(1, quantityPerFee),
                UnitPrice = unit,
                LineTotal = Math.Max(1, quantityPerFee) * unit
            });
        }

        //recalculate subtotal, total and gst. Then save
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
        
        quote.Discount = 0m; //Add discount later/.
        quote.GstAmount = Math.Round(subtotal * 0.10m, 2);
        
        quote.Total = (quote.Subtotal + quote.GstAmount - quote.Discount);
    }
    
    
}