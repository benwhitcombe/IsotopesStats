using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Authorization;
using IsotopesStats.Data;
using IsotopesStats.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor().AddCircuitOptions(options => { options.DetailedErrors = true; });
builder.Services.AddScoped<StatsRepository>();
builder.Services.AddScoped<StatsService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddAuthorizationCore();

builder.Services.AddScoped<SharedSessionState>();
builder.Services.AddScoped<PlayerStatsState>();
builder.Services.AddScoped<GameStatsState>();

// Initialize the database
await DatabaseInitializer.InitializeAsync();

WebApplication app = builder.Build();

// Seed Data
try 
{
    Console.WriteLine("Initializing database and seeding data...");
    await DataSeeder.SeedDataAsync();
    
    // Seed Admin User
    using (IServiceScope scope = app.Services.CreateScope())
    {
        AuthService authService = scope.ServiceProvider.GetRequiredService<AuthService>();
        List<IsotopesStats.Models.User> users = await authService.GetUsersAsync();
        if (!users.Any())
        {
            await authService.RegisterAsync("admin@isotopes.com", "Admin123!", true);
            Console.WriteLine("Initial admin user created: admin@isotopes.com / Admin123!");
        }
    }
    
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
