using interport.Models;
using Microsoft.EntityFrameworkCore;

namespace interport.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<QuotationRequest> QuotationRequests => Set<QuotationRequest>();
    public DbSet<Quotation> Quotations => Set<Quotation>();
    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

        // Customers
        b.Entity<Customer>().HasData(
            new Customer { Id = 1, FirstName = "John", LastName = "Doe", Email = "ops@ridgewater.com", Phone = "0400 111 222", Address = "72 Street", PasswordHash = "abcdefg"},
            new Customer { Id = 2, FirstName = "Bob", LastName = "Smith", Email = "logistics@sunrise.com", Phone = "0400 333 444", Address = "75 Street", PasswordHash = "abcdefg"}
        );

        // QuotationRequests
        b.Entity<QuotationRequest>().HasData(
            new QuotationRequest
            {
                Id = 1001,
                CustomerId = 1,
                Source = "Toowoomba, QLD",
                Destination = "Port of Brisbane, QLD",
                NumberOfContainers = 2,
                NatureOfPackage = "Pallets",
                NatureOfJob = "Export",
                CreatedUtc = DateTime.UtcNow.AddDays(-5)
            },
            new QuotationRequest
            {
                Id = 1002,
                CustomerId = 2,
                Source = "Rockhampton, QLD",
                Destination = "Brisbane, QLD",
                NumberOfContainers = 1,
                NatureOfPackage = "Machinery",
                NatureOfJob = "Import",
                CreatedUtc = DateTime.UtcNow.AddDays(-2)
            }
        );

        // Quotations (FK to QuotationRequests)
        b.Entity<Quotation>().HasData(
            new Quotation
            {
                Id = 5001,
                QuotationRequestId = 1001,
                ContainerType = "20ft",
                ScopeDescription = "Export two 20ft containers ex Toowoomba to Port of Brisbane",
                ItemCharges = 1800m,
                DepotCharges = 250m,
                LclDeliveryCharges = 0m,
                Status = "Pending",
                DateIssuedUtc = DateTime.UtcNow.AddDays(-3)
            }
        );
    }
}