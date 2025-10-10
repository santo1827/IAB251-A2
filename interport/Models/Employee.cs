namespace interport.Models;

public class Employee
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string FamilyName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string? EmployeeType { get; set; }
    public string Address { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
}