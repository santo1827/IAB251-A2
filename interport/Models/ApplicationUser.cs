using Microsoft.AspNetCore.Identity;

namespace interport.Models;

public class ApplicationUser : IdentityUser
{
    public string? DisplayName { get; set; }
    public string? OrgRole { get; set; }
}