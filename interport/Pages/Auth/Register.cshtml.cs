using System.ComponentModel.DataAnnotations;
using interport.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace interport.Pages.Auth;

public class RegisterModel : PageModel
{
    private AppDbContext dB;
    public RegisterModel(AppDbContext db) => dB = db;

            [BindProperty] public InputModel Input { get; set; } = new();

        public class InputModel
        {
            [Required] public string Role { get; } = "";            // "Customer" or "Employee"

            [Required] public string FirstName { get; set; } = "";
            [Required] public string LastName { get; set; } = "";

            [Required, EmailAddress] public string Email { get; set; } = "";

            [Required] public string Phone { get; set; } = "";
            [Required] public string Address { get; set; } = "";

            // Customer-only
            public string? CompanyName { get; }

            // Employee-only (comes from <select>)
            public string? EmployeeType { get; }                     // e.g. "Quotation Officer"

            [Required, DataType(DataType.Password)]
            public string Password { get; } = "";
        }

        public void OnGet() {}

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            // 1) Enforce unique email across both tables (simple prototype rule)
            bool emailInUse = await _db.Customers.AnyAsync(c => c.Email == Input.Email)
                           || await _db.Employees.AnyAsync(e => e.Email == Input.Email);
            if (emailInUse)
            {
                ModelState.AddModelError(nameof(Input.Email), "Email is already registered.");
                return Page();
            }

            // 2) Hash password
            var passwordHash = HashPassword(Input.Password);

            // 3) Branch by role and persist
            if (string.Equals(Input.Role, "Customer", StringComparison.OrdinalIgnoreCase))
            {
                var customer = new Customer
                {
                    FirstName   = Input.FirstName,
                    LastName    = Input.LastName,
                    Email       = Input.Email,
                    Phone       = Input.Phone,
                    Address     = Input.Address,
                    CompanyName = Input.CompanyName,
                    PasswordHash = passwordHash
                };
                _db.Customers.Add(customer);
            }
            else if (string.Equals(Input.Role, "Employee", StringComparison.OrdinalIgnoreCase))
            {
                var type = MapEmployeeType(Input.EmployeeType);
                var employee = new Employee
                {
                    FirstName    = Input.FirstName,
                    LastName     = Input.LastName,
                    Email        = Input.Email,
                    Phone        = Input.Phone,
                    Address      = Input.Address,
                    EmployeeType = type,
                    PasswordHash = passwordHash
                };
                _db.Employees.Add(employee);
            }
            else
            {
                ModelState.AddModelError(nameof(Input.Role), "Please choose Customer or Employee.");
                return Page();
            }

            await _db.SaveChangesAsync();

            // 4) Redirect to Login (or sign-in immediately if you prefer)
            return RedirectToPage("/Auth/Login");
        }

        private static EmployeeType MapEmployeeType(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return EmployeeType.Manager; // default if none selected
            // Your <select> uses labels with spaces (e.g., "Quotation Officer"), map them:
            return raw.Trim().ToLower() switch
            {
                "admin"               => EmployeeType.Admin,
                "quotation officer"   => EmployeeType.QuotationOfficer,
                "booking officer"     => EmployeeType.BookingOfficer,
                "warehouse officer"   => EmployeeType.WarehouseOfficer,
                "manager"             => EmployeeType.Manager,
                "cio"                 => EmployeeType.CIO,
                _                     => EmployeeType.Manager
            };
        }
    
}