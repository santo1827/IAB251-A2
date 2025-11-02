using System.ComponentModel.DataAnnotations;
using interport.Data;
using interport.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace interport.Areas.Identity.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly AppDbContext _db;

    public RegisterModel(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<IdentityRole> roleManager,
        AppDbContext db)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _db = db;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public class InputModel
    {
        [Required] public string Role { get; set; } = "";
        [Required] public string FirstName { get; set; } = "";
        [Required] public string LastName { get; set; } = "";
        [Required, EmailAddress] public string Email { get; set; } = "";
        [Required, DataType(DataType.Password)] public string Password { get; set; } = "";
        [Required] public string Phone { get; set; } = "";
        [Required] public string Address { get; set; } = "";
    }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var user = new ApplicationUser
        {
            UserName = Input.Email,
            Email = Input.Email,
            DisplayName = $"{Input.FirstName} {Input.LastName}",
            OrgRole = Input.Role.Equals("Employee", StringComparison.OrdinalIgnoreCase)
                ? "QuotationOfficer" : "Customer"
        };

        var result = await _userManager.CreateAsync(user, Input.Password);
        if (!result.Succeeded)
        {
            foreach (var e in result.Errors)
                ModelState.AddModelError(string.Empty, e.Description);
            return Page();
        }

        await _userManager.AddToRoleAsync(user, user.OrgRole ?? "Customer");

        // Create linked domain record
        if (user.OrgRole == "Customer")
        {
            _db.Customers.Add(new Customer
            {
                IdentityUserId = user.Id,
                FirstName = Input.FirstName,
                LastName = Input.LastName,
                Email = Input.Email,
                Phone = Input.Phone,
                Address = Input.Address,
                Country = "" // optional field
            });
        }
        else
        {
            _db.Employees.Add(new Employee
            {
                FirstName = Input.FirstName,
                FamilyName = Input.LastName,
                Email = Input.Email,
                Phone = Input.Phone,
                Address = Input.Address,
                EmployeeType = "Quotation Officer",
                CreatedUtc = DateTime.UtcNow
            });
        }

        await _db.SaveChangesAsync();

        // optional: sign in immediately
        await _signInManager.SignInAsync(user, isPersistent: false);

        return RedirectToPage("/Index");
    }
}
