namespace interport.Models;

public class Enums
{
    public enum EmployeeType
    {
    Admin,
    QuotationOfficer,
    BookingOfficer,
    WarehouseWorker, 
    Manager,
    Cio,
    }


    public enum QuoteStatus
    {
        Draft, 
        SubmittedToCustomer, 
        AcceptedByCustomer, 
        RejectedByCustomer
    }
    
    
    public enum RequestStatus { 
        Pending, 
        Approved, 
        Rejected 
    }

    public enum ContainerType
    {
        TwentyFt, 
        FortyFt, 
        LCL
    }
    
}