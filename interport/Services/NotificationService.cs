using interport.Data;
using interport.Models;
using Microsoft.EntityFrameworkCore;

namespace interport.Services;

public class NotificationService : INotificationService
{
    private readonly AppDbContext _database;
    public NotificationService(AppDbContext db)
    {
        _database = db;
    }
    
    
    //Make a new notification for a xcustomer.
    public async Task CreateForCustomer(int customerId, string title, string? message = null, string? linkUrl = null)
    {
        
        _database.Notifications.Add(new Notification {
            
            CustomerId = customerId,
            Title = title.Trim(),
            Message = string.IsNullOrWhiteSpace(message) ? null : message.Trim(),
            LinkUrl = string.IsNullOrWhiteSpace(linkUrl) ? null : linkUrl.Trim(),
            CreatedUtc = DateTime.UtcNow
        });
        await _database.SaveChangesAsync();
    }
    
    //Notification for an employee
    public async Task CreateForEmployee(int employeeId, string title, string? message = null, string? linkUrl = null)
    {
        _database.Notifications.Add(new Notification {
            EmployeeId = employeeId,
            Title = title.Trim(),
            Message = string.IsNullOrWhiteSpace(message) ? null : message.Trim(),
            LinkUrl = string.IsNullOrWhiteSpace(linkUrl) ? null : linkUrl.Trim(),
            CreatedUtc = DateTime.UtcNow
        });
        await _database.SaveChangesAsync();
    }
    
    
    
    //count notifications where read is false and customer id matches.
    public Task<int> CountUnreadForCustomer(int customerId)
    {
        return _database.Notifications.CountAsync(notification => notification.CustomerId == customerId && !notification.IsRead);
    }
//count notifications where read is false and employee id matches.
    public Task<int> CountUnreadForEmployee(int employeeId)
    {
        return _database.Notifications.CountAsync(notification => notification.EmployeeId == employeeId && !notification.IsRead);
    }
    
    
    
    
    
    //Show newest notifications for a customer
    public Task<List<Notification>> ListForCustomerAsync(int customerId, int take = 20)
        => _database.Notifications
            .Where(notification => notification.CustomerId == customerId)
            .OrderByDescending(notification => notification.CreatedUtc)
            .Take(take).ToListAsync();

    //show employees most newest notifications. Mirror of the above customer method=
    public Task<List<Notification>> ListForEmployeeAsync(int employeeId, int take = 20)
        => _database.Notifications
            .Where(notification => notification.EmployeeId == employeeId)
            .OrderByDescending(notification => notification.CreatedUtc)
            .Take(take).ToListAsync();
    
    
    
    //set a notification as read.
    public async Task MarkReadAsync(int id)
    {
        var notification = await _database.Notifications.FindAsync(id);
        if (notification is null) return; // dont do anything if notification is null
        notification.IsRead = true;
        await _database.SaveChangesAsync();
    }
}