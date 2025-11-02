using interport.Models;

namespace interport.Services;

//Notification async methods.
public interface INotificationService
{
    Task CreateForCustomer(int customerId, string title, string? message = null, string? linkUrl = null);
    Task CreateForEmployee(int employeeId, string title, string? message = null, string? linkUrl = null);

    Task<int> CountUnreadForCustomer(int customerId);
    Task<int> CountUnreadForEmployee(int employeeId);

    Task<List<Notification>> ListForCustomerAsync(int customerId, int take = 20);
    Task<List<Notification>> ListForEmployeeAsync(int employeeId, int take = 20);
    
    Task MarkReadAsync(int id);
}