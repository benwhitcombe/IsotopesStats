using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using IsotopesStats.Models;
using Supabase;
using Postgrest;
using Postgrest.Responses;

namespace IsotopesStats.Services;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly Supabase.Client _supabase;
    private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());

    public CustomAuthenticationStateProvider(Supabase.Client supabase)
    {
        _supabase = supabase;

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

            // Fetch custom roles/permissions from our DB for this Supabase user
            List<UserRole> roles = await GetUserRolesForUserAsync(session.User.Id);
            return new AuthenticationState(CreateClaimsPrincipal(session.User, roles));
        }
        catch
        {
            return new AuthenticationState(_anonymous);
        }
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
            ModeledResponse<UserRole> response = await _supabase.Postgrest
                .Table<UserRole>()
                .Select("id, name, isdeleted, rolepermissions(permissions(id, name))")
                .Filter("useruserroles.userid", Constants.Operator.Equals, supabaseUserId)
                .Get();

            return response.Models;
        }
        catch
        {
            return new List<UserRole>();
        }
    }
}
