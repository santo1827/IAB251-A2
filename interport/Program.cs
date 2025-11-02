using interport.Data;
using interport.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using interport.Models;

var builder = WebApplication.CreateBuilder(args);

// Explicit DB path -> interport.db at project root
var dbPath = Path.Combine(builder.Environment.ContentRootPath, "interport.db");
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite($"Data Source={dbPath}"));

// Add service
builder.Services.AddScoped<IQuoteLineService, QuoteLineService>();
builder.Services.AddScoped<IRateScheduleService, RateScheduleService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

// -- Authentication Section --
builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequireDigit = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 6;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();

// Razor Pages
builder.Services.ConfigureApplicationCookie(c =>
{
    c.LoginPath = "/Identity/Account/Login";
    c.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/");               // default: require auth
    options.Conventions.AllowAnonymousToPage("/Index");     // public home
});




static async Task SeedAuthAsync(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    // 1) Ensure roles exist
    foreach (var role in new[] { "Customer", "QuotationOfficer" })
        if (!await roleMgr.RoleExistsAsync(role))
            await roleMgr.CreateAsync(new IdentityRole(role));

    // 2) Ensure an initial officer user exists
    var email = "officer@interport.local";
    var user = await userMgr.FindByEmailAsync(email);
    if (user is null)
    {
        user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            DisplayName = "Quotation Officer",
            OrgRole = "QuotationOfficer"
        };
        // Temp dev password; rotate in prod
        var createResult = await userMgr.CreateAsync(user, "ChangeMe_123!");
        if (!createResult.Succeeded)
            throw new InvalidOperationException(string.Join("; ", createResult.Errors.Select(e => e.Description)));

        await userMgr.AddToRoleAsync(user, "QuotationOfficer");
    }
}

var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

await SeedAuthAsync(app.Services);

app.MapStaticAssets();
app.MapRazorPages().WithStaticAssets();

app.Run();
