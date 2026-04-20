using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using IsotopesStats.Models;
using Supabase;
using IsotopesStats.Domain.Interfaces;

namespace IsotopesStats.Services;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly Supabase.Client _supabase;
    private readonly IServiceProvider _serviceProvider;
    private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());

    public CustomAuthenticationStateProvider(Supabase.Client supabase, IServiceProvider serviceProvider)
    {
        _supabase = supabase;
        _serviceProvider = serviceProvider;

        // Listen for Supabase Auth State Changes
        _supabase.Auth.AddStateChangedListener((sender, state) =>
        {
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        });
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            Supabase.Gotrue.Session? session = _supabase.Auth.CurrentSession;

            if (session == null || session.User == null || string.IsNullOrEmpty(session.User.Id))
                return new AuthenticationState(_anonymous);

            // Use AuthService to check if user is deleted and get roles
            using IServiceScope scope = _serviceProvider.CreateScope();
            AuthService authService = scope.ServiceProvider.GetRequiredService<AuthService>();

            User? dbUser = await authService.GetUserByIdAsync(session.User.Id);
            if (dbUser != null && dbUser.IsDeleted)
            {
                return new AuthenticationState(_anonymous);
            }
            
            List<UserRole> roles = await GetUserRolesForUserAsync(session.User.Id);
            return new AuthenticationState(CreateClaimsPrincipal(session.User, roles));
        }
        catch (Exception ex)
        {
            Console.WriteLine("Auth Error: " + ex.Message);
            return new AuthenticationState(_anonymous);
        }
    }

    public void NotifyStateChanged()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task UpdateAuthenticationState(User? userSession)
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        await Task.CompletedTask;
    }

    private ClaimsPrincipal CreateClaimsPrincipal(Supabase.Gotrue.User user, List<UserRole> roles)
    {
        List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id ?? ""),
            new Claim(ClaimTypes.Name, user.Email ?? ""),
            new Claim(ClaimTypes.Email, user.Email ?? "")
        };

        foreach (UserRole role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role.Name));
            foreach (Permission permission in role.Permissions)
            {
                claims.Add(new Claim("Permission", permission.Name));
            }
        }

        return new ClaimsPrincipal(new ClaimsIdentity(claims, "SupabaseAuth"));
    }

    private async Task<List<UserRole>> GetUserRolesForUserAsync(string supabaseUserId)
    {
        if (string.IsNullOrEmpty(supabaseUserId)) return new List<UserRole>();

        try 
        {
            using IServiceScope scope = _serviceProvider.CreateScope();
            IAuthRepository authRepository = scope.ServiceProvider.GetRequiredService<IAuthRepository>();
            return await authRepository.GetUserRolesForUserAsync(supabaseUserId);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Auth Error: " + ex.Message);
            return new List<UserRole>();
        }
    }
}
