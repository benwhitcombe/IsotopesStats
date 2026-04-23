using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using IsotopesStats;
using IsotopesStats.Services;
using IsotopesStats.Models;
using Supabase;
using IsotopesStats.Domain.Interfaces;
using SupabaseRepository.Extensions;

WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Add Supabase Repositories (Isolated)
string supabaseUrl = builder.Configuration["Supabase:Url"] ?? throw new Exception("Supabase URL is missing");
string supabaseKey = builder.Configuration["Supabase:Key"] ?? throw new Exception("Supabase Key is missing");
builder.Services.AddSupabaseRepositories(supabaseUrl, supabaseKey);

// Core Services
builder.Services.AddScoped<SessionStorageService>();
builder.Services.AddScoped<CustomAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<CustomAuthenticationStateProvider>());

// Authorization policies
builder.Services.AddAuthorizationCore(options =>
{
    options.AddPolicy("Manage Games", policy => policy.RequireClaim("Permission", "Manage Games"));
    options.AddPolicy("Manage Players", policy => policy.RequireClaim("Permission", "Manage Players"));
    options.AddPolicy("Manage Opponents", policy => policy.RequireClaim("Permission", "Manage Opponents"));
    options.AddPolicy("Manage Seasons", policy => policy.RequireClaim("Permission", "Manage Seasons"));
    options.AddPolicy("Manage Users", policy => policy.RequireClaim("Permission", "Manage Users"));
    options.AddPolicy("Manage Roles", policy => policy.RequireClaim("Permission", "Manage Roles"));
    options.AddPolicy("Manage Logs", policy => policy.RequireClaim("Permission", "Manage Logs"));
});

// State Management
builder.Services.AddScoped<SharedSessionState>();
builder.Services.AddScoped<PlayerStatsState>();
builder.Services.AddScoped<GameStatsState>();
builder.Services.AddScoped<PersistenceManager>();

// HttpClient (standard for WASM)
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

WebAssemblyHost host = builder.Build();

// Initialize App State Persistence
PersistenceManager persistenceManager = host.Services.GetRequiredService<PersistenceManager>();
await persistenceManager.InitializeAsync();

// Initialize Supabase Auth Session
Supabase.Client supabase = host.Services.GetRequiredService<Supabase.Client>();
try 
{
    // Explicitly load the session from the persistence handler
    supabase.Auth.LoadSession();
    // Initialize the client (triggers token refresh check)
    await supabase.InitializeAsync();
}
catch (Exception ex) 
{ 
    Console.WriteLine($"Auth initialization failed: {ex.Message}"); 
}

// Ensure the UI reflects the loaded auth state
CustomAuthenticationStateProvider authProvider = (CustomAuthenticationStateProvider)host.Services.GetRequiredService<AuthenticationStateProvider>();
authProvider.NotifyStateChanged();

await host.RunAsync();

