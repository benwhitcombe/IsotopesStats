using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using IsotopesStats;
using IsotopesStats.Services;
using IsotopesStats.Models;
using Supabase;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Add Supabase Client
var supabaseUrl = builder.Configuration["Supabase:Url"] ?? throw new Exception("Supabase URL is missing");
var supabaseKey = builder.Configuration["Supabase:Key"] ?? throw new Exception("Supabase Key is missing");

var options = new SupabaseOptions
{
    AutoRefreshToken = true,
    AutoConnectRealtime = true
};

builder.Services.AddScoped(_ => new Supabase.Client(supabaseUrl, supabaseKey, options));

// Core Services
builder.Services.AddScoped<StatsService>();
builder.Services.AddScoped<AuthService>();
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

// HttpClient (standard for WASM)
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();
