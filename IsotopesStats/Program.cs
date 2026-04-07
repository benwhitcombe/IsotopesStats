using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Authorization;
using IsotopesStats.Data;
using IsotopesStats.Services;
using IsotopesStats.Models;
using Microsoft.Data.Sqlite;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor().AddCircuitOptions(options => { options.DetailedErrors = true; });
builder.Services.AddScoped<StatsRepository>();
builder.Services.AddScoped<StatsService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<CustomAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<CustomAuthenticationStateProvider>());
builder.Services.AddAuthorizationCore(options =>
{
    options.AddPolicy("Manage Games", policy => policy.RequireClaim("Permission", "Manage Games"));
    options.AddPolicy("Manage Players", policy => policy.RequireClaim("Permission", "Manage Players"));
    options.AddPolicy("Manage Opponents", policy => policy.RequireClaim("Permission", "Manage Opponents"));
    options.AddPolicy("Manage Seasons", policy => policy.RequireClaim("Permission", "Manage Seasons"));
    options.AddPolicy("Manage Users", policy => policy.RequireClaim("Permission", "Manage Users"));
    options.AddPolicy("Manage Roles", policy => policy.RequireClaim("Permission", "Manage Roles"));
});

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
    
    // Seed Permissions and Roles
    using (IServiceScope scope = app.Services.CreateScope())
    {
        AuthService authService = scope.ServiceProvider.GetRequiredService<AuthService>();
        
        // 1. Seed Permissions
        List<Permission> existingPermissions = await authService.GetPermissionsAsync();
        string[] permissionNames = { "Manage Games", "Manage Players", "Manage Opponents", "Manage Seasons", "Manage Users", "Manage Roles" };
        
        using (SqliteConnection connection = new SqliteConnection("Data Source=Data/IsotopesStats.db"))
        {
            await connection.OpenAsync();
            foreach (string pName in permissionNames)
            {
                if (!existingPermissions.Any(p => p.Name == pName))
                {
                    SqliteCommand cmd = connection.CreateCommand();
                    cmd.CommandText = "INSERT INTO Permissions (Name) VALUES ($name)";
                    cmd.Parameters.AddWithValue("$name", pName);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
        
        // Refresh permissions
        List<Permission> allPermissions = await authService.GetPermissionsAsync();
        Permission pGames = allPermissions.First(p => p.Name == "Manage Games");
        Permission pPlayers = allPermissions.First(p => p.Name == "Manage Players");
        Permission pOpponents = allPermissions.First(p => p.Name == "Manage Opponents");
        Permission pSeasons = allPermissions.First(p => p.Name == "Manage Seasons");
        Permission pUsers = allPermissions.First(p => p.Name == "Manage Users");
        Permission pRoles = allPermissions.First(p => p.Name == "Manage Roles");

        // 2. Seed Roles
        List<UserRole> existingRoles = await authService.GetUserRolesAsync();
        
        if (!existingRoles.Any(r => r.Name == "Administrator"))
        {
            await authService.AddUserRoleAsync(new UserRole { Name = "Administrator", Permissions = allPermissions });
        }
        if (!existingRoles.Any(r => r.Name == "Team Rep"))
        {
            await authService.AddUserRoleAsync(new UserRole { Name = "Team Rep", Permissions = new List<Permission> { pGames, pPlayers, pOpponents, pSeasons } });
        }
        if (!existingRoles.Any(r => r.Name == "Scorekeeper"))
        {
            await authService.AddUserRoleAsync(new UserRole { Name = "Scorekeeper", Permissions = new List<Permission> { pGames } });
        }
        if (!existingRoles.Any(r => r.Name == "Player"))
        {
            await authService.AddUserRoleAsync(new UserRole { Name = "Player", Permissions = new List<Permission>() });
        }

        // 3. Seed Admin User
        List<IsotopesStats.Models.User> users = await authService.GetUsersAsync();
        if (!users.Any())
        {
            List<UserRole> roles = await authService.GetUserRolesAsync();
            UserRole adminRole = roles.First(r => r.Name == "Administrator");
            await authService.RegisterAsync("admin@isotopes.com", "Admin123!", new List<int> { adminRole.Id });
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
