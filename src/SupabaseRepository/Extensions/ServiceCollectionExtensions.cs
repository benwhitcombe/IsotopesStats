using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Supabase;
using IsotopesStats.Domain.Interfaces;
using IsotopesStats.SupabaseRepository.Repositories;
using IsotopesStats.SupabaseRepository.Auth;

namespace IsotopesStats.SupabaseRepository.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSupabaseRepositories(this IServiceCollection services, string url, string key)
    {
        // Add Supabase Client
        services.AddScoped(sp => 
        {
            SupabaseOptions options = new SupabaseOptions
            {
                AutoRefreshToken = true,
                AutoConnectRealtime = true,
                SessionHandler = new SupabaseSessionPersistence(sp.GetRequiredService<IJSRuntime>())
            };
            return new Supabase.Client(url, key, options);
        });

        // Register Repositories
        services.AddScoped<IStatsRepository, StatsRepository>();
        services.AddScoped<IAuthRepository, AuthRepository>();

        return services;
    }
}
