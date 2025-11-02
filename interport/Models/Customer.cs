namespace interport.Models;

public class Customer
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;             // username
    public string Phone { get; set; } = null!;
    public string? CompanyName { get; set; }

    public string PasswordHash { get; set; } = null!;      // store only hashes
    
    public string Country { get; set; }
    
    public string Address { get; set; } = null!;
    
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
}