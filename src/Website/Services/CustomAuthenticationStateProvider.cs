using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using IsotopesStats.Domain.Models;
using Supabase;
using IsotopesStats.Domain.Interfaces;

namespace IsotopesStats.Website.Services;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly Supabase.Client _supabase;
    private readonly IAuthRepository _authRepository;
    private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());

    public CustomAuthenticationStateProvider(Supabase.Client supabase, IAuthRepository authRepository)
    {
        _supabase = supabase;
        _authRepository = authRepository;

        // Listen for Supabase Auth State Changes (SignIn, SignOut, Token Refresh)
        _supabase.Auth.AddStateChangedListener((sender, state) =>
        {
            NotifyStateChanged();
        });
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            Supabase.Gotrue.Session? session = _supabase.Auth.CurrentSession;

            if (session == null || session.User == null || string.IsNullOrEmpty(session.User.Id))
                return new AuthenticationState(_anonymous);

            User? dbUser = await _authRepository.GetUserByIdAsync(session.User.Id);
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
            return await _authRepository.GetUserRolesForUserAsync(supabaseUserId);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Auth Error: " + ex.Message);
            return new List<UserRole>();
        }
    }
}
