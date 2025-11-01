﻿using interport.Models;
using Microsoft.EntityFrameworkCore;

namespace interport.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<QuotationRequest> QuotationRequests => Set<QuotationRequest>();
    public DbSet<Quotation> Quotations => Set<Quotation>();
    
    public DbSet<QuoteLine> QuotationLines => Set<QuoteLine>();
    
    //Configure database 
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);


        //Set up relationship between Quotation and Lines and deletion behaviour
        builder.Entity<Quotation>()
            .HasMany(quotation => quotation.Lines)
            .WithOne(quoteLine => quoteLine.Quotation!)
            .HasForeignKey(quoteLine => quoteLine.QuotationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Ensure quote number is unique
        builder.Entity<Quotation>()
            .HasIndex(q => q.QuoteNumber)
            .IsUnique();

        //Ensure column type is correct.
        builder.Entity<Quotation>().Property(p => p.Subtotal).HasColumnType("decimal(16,4)");
        builder.Entity<Quotation>().Property(p => p.Discount).HasColumnType("decimal(16,4)");
        builder.Entity<Quotation>().Property(p => p.Total).HasColumnType("decimal(16,4)");
        
        
        builder.Entity<QuoteLine>().Property(p => p.UnitPrice).HasColumnType("decimal(16,4)");
        builder.Entity<QuoteLine>().Property(p => p.LineTotal).HasColumnType("decimal(16,4)");
    }
}
