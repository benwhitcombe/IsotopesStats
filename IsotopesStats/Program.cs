using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using IsotopesStats.Data;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor().AddCircuitOptions(options => { options.DetailedErrors = true; });
builder.Services.AddScoped<StatsService>();

// Initialize the database
DatabaseInitializer.Initialize();

WebApplication app = builder.Build();

// Seed 2025 Data
try 
{
    Console.WriteLine("Initializing database and seeding data...");
    DataSeeder.Seed2025Data();
    Console.WriteLine("Database initialization complete.");
}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred seeding the DB: {ex.Message}");
}

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

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
