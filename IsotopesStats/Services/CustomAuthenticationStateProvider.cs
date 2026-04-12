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
        catch (Exception ex)
        {
            Console.WriteLine("Auth Error: " + ex.Message);
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
            ModeledResponse<UserUserRoles> urResponse = await _supabase.From<UserUserRoles>().Where(x => x.UserId == supabaseUserId).Get();
            List<int> roleIds = urResponse.Models.Select(x => x.RoleId).ToList();

            if (!roleIds.Any()) return new List<UserRole>();

            ModeledResponse<UserRole> rolesResponse = await _supabase.From<UserRole>().Get();
            List<UserRole> roles = rolesResponse.Models.Where(r => roleIds.Contains(r.Id)).ToList();

            ModeledResponse<RolePermission> rpResponse = await _supabase.From<RolePermission>().Get();
            List<int> permissionIds = rpResponse.Models.Where(rp => roleIds.Contains(rp.RoleId)).Select(x => x.PermissionId).Distinct().ToList();

            if (permissionIds.Any())
            {
                ModeledResponse<Permission> permResponse = await _supabase.From<Permission>().Get();
                List<Permission> perms = permResponse.Models;

                foreach (UserRole role in roles)
                {
                    List<int> rolePermIds = rpResponse.Models.Where(x => x.RoleId == role.Id).Select(x => x.PermissionId).ToList();
                    role.Permissions = perms.Where(p => rolePermIds.Contains(p.Id)).ToList();
                }
            }

            return roles;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Auth Error: " + ex.Message);
            return new List<UserRole>();
        }
    }
}
