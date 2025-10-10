using System.ComponentModel.DataAnnotations;
using interport.Data;
using interport.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace interport.Pages.Auth;

public class RegisterModel : PageModel
{
    private AppDbContext dB;
    public RegisterModel(AppDbContext db) => dB = db;

    [BindProperty] public InputModel FormData { get; set; } = new();

    //HTML form data
        public class InputModel
        {
            [Required] public string Role { get; } = "";
            [Required] public string FirstName { get;} = "";
            [Required] public string LastName { get;} = "";
            [Required, EmailAddress] public string Email { get;} = "";

            [Required] public string Phone { get; } = "";
            [Required] public string Address { get;  } = "";

            // Customer-only
            public string? CompanyName { get; }

            
            // Employee only
            public string? EmployeeType { get; }

            [Required, DataType(DataType.Password)]
            public string Password { get; } = "";
        }

        public void OnGet() {}

        //On post or form submitted.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            
            
            bool emailInUse = await dB.Customers.AnyAsync(customer => customer.Email == FormData.Email) 
                              || await dB.Employees.AnyAsync(employee => employee.Email == FormData.Email);
            if (emailInUse)
            {
                ModelState.AddModelError(nameof(FormData.Email), "Email is already registered.");
                return Page();
            }


            if (string.Equals(FormData.Role, "Customer", StringComparison.OrdinalIgnoreCase))
            {
                var newCustomer = new Customer
                {
                    FirstName   = FormData.FirstName,
                    LastName    = FormData.LastName,
                    Email       = FormData.Email,
                    Phone       = FormData.Phone,
                    Address     = FormData.Address,
                    CompanyName = FormData.CompanyName,
                    PasswordHash = FormData.Password
                };
                dB.Customers.Add(newCustomer);
            }
            else if (string.Equals(FormData.Role, "Employee", StringComparison.OrdinalIgnoreCase))
            {
                var type = MapEmployeeType(FormData.EmployeeType);
                Employee newEmployee = new Employee
                {
                    FirstName    = FormData.FirstName,
                    FamilyName     = FormData.LastName,
                    Email        = FormData.Email,
                    Phone        = FormData.Phone,
                    Address      = FormData.Address,
                    EmployeeType = FormData.EmployeeType,
                    PasswordHash = FormData.Password,
                    CreatedUtc   = DateTime.UtcNow,
                };
                dB.Employees.Add(newEmployee);
            }
            else
            {
                ModelState.AddModelError(nameof(FormData.Role), "Please choose Customer or Employee.");
                return Page();
            }

            await dB.SaveChangesAsync();

            // ****Implement redirect to home screen.
            return RedirectToPage("/Auth/Register");
        }

        private static Enums.EmployeeType MapEmployeeType(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return Enums.EmployeeType.WarehouseWorker; // default if something isnt selected
            // map logical enums to form types:
            return raw.Trim().ToLower() switch
            {
                "admin"               => Enums.EmployeeType.Admin,
                "quotation officer"   => Enums.EmployeeType.QuotationOfficer,
                "booking officer"     => Enums.EmployeeType.BookingOfficer,
                "warehouse officer"   => Enums.EmployeeType.WarehouseWorker,
                "manager"             => Enums.EmployeeType.Manager,
                "cio"                 => Enums.EmployeeType.Cio,
                _                     => Enums.EmployeeType.Manager
            };
        }
    
}