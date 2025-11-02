using interport.Models;

namespace interport.Services;


public interface IRateScheduleService
{
    //Get price for a fee type depending on container size
    decimal GetUnitPrice(Enums.ContainerType container, Enums.FeeType feeType);
    
    //Get user friendly label based on fee  type
    string GetLabel(Enums.FeeType feeType);
    
    //Get all the fee types.
    IEnumerable<Enums.FeeType> AllFees();
}