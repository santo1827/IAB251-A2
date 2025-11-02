using interport.Models;

namespace interport.Services;

public class RateScheduleService : IRateScheduleService
{
    //Labels for UI descriptions.
    private static readonly Dictionary<Enums.FeeType, string> Labels = new()
    {
        { Enums.FeeType.WharfBooking, "Wharf Booking fee" },
        { Enums.FeeType.LiftOnOff, "Lift on/Lift off" },
        { Enums.FeeType.Fumigation, "Fumigation Fee" },
        { Enums.FeeType.LclDeliveryDepot, "LCL Delivery Depot fee" },
        { Enums.FeeType.TailgateInspection, "Tailgate inspection" },
        { Enums.FeeType.StorageFee, "Storage Fee" },
        { Enums.FeeType.FacilityFee, "Facility Fee" },
        { Enums.FeeType.WharfInspection, "Wharf Inspection" }
    };
    
    
    // Fee rates dict. Key is (ContainerType, FeeType)  and value is decimal price
    private static readonly Dictionary<(Enums.ContainerType, Enums.FeeType), decimal> Prices = new() {
        
        { (Enums.ContainerType.TwentyFt, Enums.FeeType.WharfBooking),       60m  },
        { (Enums.ContainerType.FortyFt,  Enums.FeeType.WharfBooking),       70m  },

        { (Enums.ContainerType.TwentyFt, Enums.FeeType.LiftOnOff),          80m  },
        { (Enums.ContainerType.FortyFt,  Enums.FeeType.LiftOnOff),          120m },

        { (Enums.ContainerType.TwentyFt, Enums.FeeType.Fumigation),         220m },
        { (Enums.ContainerType.FortyFt,  Enums.FeeType.Fumigation),         280m },

        { (Enums.ContainerType.TwentyFt, Enums.FeeType.LclDeliveryDepot),   400m },
        { (Enums.ContainerType.FortyFt,  Enums.FeeType.LclDeliveryDepot),   500m },

        { (Enums.ContainerType.TwentyFt, Enums.FeeType.TailgateInspection), 120m },
        { (Enums.ContainerType.FortyFt,  Enums.FeeType.TailgateInspection), 160m },

        { (Enums.ContainerType.TwentyFt, Enums.FeeType.StorageFee),         240m },
        { (Enums.ContainerType.FortyFt,  Enums.FeeType.StorageFee),         300m },

        { (Enums.ContainerType.TwentyFt, Enums.FeeType.FacilityFee),        70m  },
        { (Enums.ContainerType.FortyFt,  Enums.FeeType.FacilityFee),        100m },

        { (Enums.ContainerType.TwentyFt, Enums.FeeType.WharfInspection),    60m  },
        { (Enums.ContainerType.FortyFt,  Enums.FeeType.WharfInspection),    90m  },
    };

    //Retrieve price for fee and container type if the input is valid.
    public decimal GetUnitPrice(Enums.ContainerType container, Enums.FeeType feeType)
    {
        if (Prices.TryGetValue((container, feeType), out var price))
        {
            return price;
        }
        throw new InvalidOperationException($"No rate defined for {container} /  {feeType}");
    }
    
    //Get the ui friendly label for fee type method.
    public string GetLabel(Enums.FeeType feeType) => Labels[feeType];
    
    //Get all the fee types 
    public IEnumerable<Enums.FeeType> AllFees() => Labels.Keys;
    
    
}