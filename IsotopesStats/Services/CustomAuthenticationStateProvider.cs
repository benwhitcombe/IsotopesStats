using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using IsotopesStats.Models;
using Supabase;

namespace IsotopesStats.Services;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly Client _supabase;
    private readonly AuthService _authService;
    private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());

    public CustomAuthenticationStateProvider(Client supabase, AuthService authService)
    {
        _supabase = supabase;
        _authService = authService;

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
            var session = _supabase.Auth.CurrentSession;

            if (session == null || session.User == null)
                return new AuthenticationState(_anonymous);

            // Fetch custom roles/permissions from our DB for this Supabase user
            var roles = await GetUserRolesForUserAsync(session.User.Id);
            return new AuthenticationState(CreateClaimsPrincipal(session.User, roles));
        }
        catch
        {
            return new AuthenticationState(_anonymous);
        }
    }

    private ClaimsPrincipal CreateClaimsPrincipal(Supabase.Gotrue.User user, List<UserRole> roles)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id ?? ""),
            new Claim(ClaimTypes.Name, user.Email ?? ""),
            new Claim(ClaimTypes.Email, user.Email ?? "")
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role.Name));
            foreach (var permission in role.Permissions)
            {
                claims.Add(new Claim("Permission", permission.Name));
            }
        }

        return new ClaimsPrincipal(new ClaimsIdentity(claims, "SupabaseAuth"));
    }

    private async Task<List<UserRole>> GetUserRolesForUserAsync(string supabaseUserId)
    {
        try 
        {
            var response = await _supabase
                .From<UserRole>()
                .Select("id, name, isdeleted, rolepermissions(permissions(id, name))")
                .Join<UserUserRoles>("id", "roleid")
                .Where<UserUserRoles>(x => x.UserId.ToString() == supabaseUserId)
                .Get();

            return response.Models;
        }
        catch
        {
            return new List<UserRole>();
        }
    }
}
