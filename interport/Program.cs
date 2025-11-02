using interport.Data;
using interport.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Razor Pages
builder.Services.AddRazorPages();

// Explicit DB path -> interport.db at project root
var dbPath = Path.Combine(builder.Environment.ContentRootPath, "interport.db");
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite($"Data Source={dbPath}"));

// Add service
builder.Services.AddScoped<IQuoteLineService, QuoteLineService>();
builder.Services.AddScoped<IRateScheduleService, RateScheduleService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

var app = builder.Build();

// Auto-create / migrate the database on boot
// using (var scope = app.Services.CreateScope())
// {
//     var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//     db.Database.Migrate();
//     // Optional: log the full path so you can find the file instantly
//     app.Logger.LogInformation("SQLite DB path: {DbPath}", dbPath);
// }

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages().WithStaticAssets();

app.Run();
