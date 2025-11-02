using System.ComponentModel.DataAnnotations;
using interport.Data;
using interport.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace interport.Web.Pages.Auth;

public class RegisterModel : PageModel
{
    private readonly UserManager<ApplicationUser> _users;
    private readonly AppDbContext _db; // kept if you later want to link domain profiles

    public RegisterModel(UserManager<ApplicationUser> users, AppDbContext db)
    {
        _users = users;
        _db = db;
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

        // Optional fields CAN exist on the form, but we will not create domain rows here
        public string? CompanyName { get; set; }    // Customer-only (ignored here)
        public string? EmployeeType { get; set; }   // Employee-only (ignored here)

        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = "";
    }

    public void OnGet() {}

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid) return Page();

        // 1) Enforce email uniqueness via Identity
        var existing = await _users.FindByEmailAsync(FormData.Email);
        if (existing is not null)
        {
            ModelState.AddModelError(nameof(FormData.Email), "Email is already registered.");
            return Page();
        }

        // 2) Map selection → Identity role name
        var role = FormData.Role.Equals("Employee", StringComparison.OrdinalIgnoreCase)
            ? "QuotationOfficer"
            : "Customer";

        // 3) Create Identity user (Identity owns credentials and hashing)
        var user = new ApplicationUser
        {
            UserName    = FormData.Email,
            Email       = FormData.Email,
            DisplayName = $"{FormData.FirstName} {FormData.LastName}",
            OrgRole     = role
        };

        var createResult = await _users.CreateAsync(user, FormData.Password);
        if (!createResult.Succeeded)
        {
            foreach (var e in createResult.Errors)
                ModelState.AddModelError(string.Empty, e.Description);
            return Page();
        }

        // 4) Attach role to the user
        var roleResult = await _users.AddToRoleAsync(user, role);
        if (!roleResult.Succeeded)
        {
            foreach (var e in roleResult.Errors)
                ModelState.AddModelError(string.Empty, e.Description);
            return Page();
        }

        // 5) Non-optional behavior: do NOT auto sign-in; redirect to Login
        return RedirectToPage("/login");
    }
}
