using LuckyApp.Data;
using LuckyApp.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add MVC services.
builder.Services.AddControllersWithViews();

// Register OLG results service.
builder.Services.AddHttpClient<OlgPick4Service>();

// Register Pick 4 calculation services.
builder.Services.AddScoped<Pick4CalculationService>();
builder.Services.AddScoped<Pick4PositionService>();

// Configure PostgreSQL database.
builder.Services.AddDbContext<LuckyAppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString(
            "DefaultConnection")
        ?? throw new InvalidOperationException(
            "Database connection string is missing.")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");

    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();