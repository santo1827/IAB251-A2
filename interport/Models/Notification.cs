using System.ComponentModel.DataAnnotations;

namespace interport.Models;

public class Notification
{
    public int Id { get; set; }

    // Set notification to belong to either a customer or an employee. NOT BOTH
    public int? CustomerId { get; set; }
    public Customer? Customer { get; set; }

    public int? EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    
    
    //Notif title
    [Required, MaxLength(200)]
    public string Title { get; set; } = "";

    //The notification message
    [MaxLength(1000)]
    public string? Message { get; set; }

    //Url to alert page
    [MaxLength(512)]
    public string? LinkUrl { get; set; }

    //Has it been read.
    public bool IsRead { get; set; } = false;
    //Time it was created.
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
}