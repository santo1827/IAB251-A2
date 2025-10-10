using interport.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();

// Data Layer

public class Customer
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string FamilyName { get; set; } = null!;
    public string Email { get; set; } = null!;             // username
    public string Phone { get; set; } = null!;
    public string? CompanyName { get; set; }
    public string Address { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;      // store only hashes
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
}

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

public class QuotationRequest
{
    public int Id { get; set; }                                // Request ID
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;
    public string Source { get; set; } = null!;
    public string Destination { get; set; } = null!;
    public int NumberOfContainers { get; set; }
    public string NatureOfPackage { get; set; } = null!;
    public string NatureOfJob { get; set; } = null!;           // import/export, pack/unpack, quarantine, etc.
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
}

public class Quotation
{
    public int Id { get; set; }                                // Quotation number
    public int QuotationRequestId { get; set; }
    public QuotationRequest QuotationRequest { get; set; } = null!;
    public string ContainerType { get; set; } = null!;
    public string ScopeDescription { get; set; } = null!;
    public decimal ItemCharges { get; set; }
    public decimal DepotCharges { get; set; }
    public decimal LclDeliveryCharges { get; set; }
    public string Status { get; set; } = "Pending";            // Accept/Reject/Pending
    public DateTime DateIssuedUtc { get; set; } = DateTime.UtcNow;
}