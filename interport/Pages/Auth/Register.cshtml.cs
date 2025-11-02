using System.ComponentModel.DataAnnotations;
using interport.Data;
using interport.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace interport.Pages.Auth;

public class RegisterModel : PageModel
{
    private readonly UserManager<ApplicationUser> _users;

    public RegisterModel(UserManager<ApplicationUser> users)
    {
        _users = users;
    }

    [BindProperty]
    public InputModel FormData { get; set; } = new();

    public class InputModel
    {
        [Required] public string Role { get; set; } = "";           // "Customer" or "Employee"
        [Required] public string FirstName { get; set; } = "";
        [Required] public string LastName  { get; set; } = "";
        [Required, EmailAddress] public string Email { get; set; } = "";
        [Required] public string Phone   { get; set; } = "";
        [Required] public string Address { get; set; } = "";
        [Required, DataType(DataType.Password)] public string Password { get; set; } = "";
    }

    public void OnGet() {}

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        var existing = await _users.FindByEmailAsync(FormData.Email);
        if (existing is not null)
        {
            ModelState.AddModelError(nameof(FormData.Email), "Email is already registered.");
            return Page();
        }

        var role = FormData.Role.Equals("Employee", StringComparison.OrdinalIgnoreCase)
            ? "QuotationOfficer" : "Customer";

        var user = new ApplicationUser
        {
            UserName    = FormData.Email,
            Email       = FormData.Email,
            DisplayName = $"{FormData.FirstName} {FormData.LastName}",
            OrgRole     = role
        };

        var create = await _users.CreateAsync(user, FormData.Password);
        if (!create.Succeeded)
        {
            foreach (var e in create.Errors)
                ModelState.AddModelError(string.Empty, e.Description);
            return Page();
        }

        var addRole = await _users.AddToRoleAsync(user, role);
        if (!addRole.Succeeded)
        {
            foreach (var e in addRole.Errors)
                ModelState.AddModelError(string.Empty, e.Description);
            return Page();
        }

        // Do not auto sign-in; send them to login (matches @page "/login")
        return Redirect("/login");
    }
}
