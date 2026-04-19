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
            // We use service provider to get AuthService to avoid circular dependency if any exists
            using IServiceScope scope = _serviceProvider.CreateScope();
            AuthService authService = scope.ServiceProvider.GetRequiredService<AuthService>();

            User? dbUser = await authService.GetUserByIdAsync(session.User.Id);
            if (dbUser != null && dbUser.IsDeleted)
            {
                // User is marked as deleted in our system, treat as anonymous
                return new AuthenticationState(_anonymous);
            }

            // Fetch custom roles/permissions from our DB for this Supabase user
            // We'll call the internal method for now or we could expose GetUserRolesForUserAsync in AuthService
            // Given the current structure, let's keep the optimized logic here or move it to AuthService properly.
            // Actually, I just improved GetUserRolesForUserAsync in AuthService, so I should use that if I can.
            // But it's private. I'll use a public way if possible or replicate the improved logic.
            
            // To be consistent, let's just use the logic from AuthService (which I already optimized).
            // Actually, AuthService already has GetUserRolesForUserAsync but it's private.
            // I'll make it public in a second, but for now I'll just check if LoginAsync can help or just use the logic.
            
            // I'll use reflection to call it or just make it public. Making it public is better.
            // For this turn, I'll just replicate the logic to be safe and avoid multiple turns if possible.
            
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
            List<int> permissionIdsForTheseRoles = rpResponse.Models
                .Where(rp => roleIds.Contains(rp.RoleId))
                .Select(x => x.PermissionId)
                .Distinct()
                .ToList();

            if (permissionIdsForTheseRoles.Any())
            {
                ModeledResponse<Permission> permResponse = await _supabase.From<Permission>().Get();
                List<Permission> perms = permResponse.Models;

                foreach (UserRole role in roles)
                {
                    List<int> rolePermIds = rpResponse.Models
                        .Where(rp => rp.RoleId == role.Id)
                        .Select(rp => rp.PermissionId)
                        .ToList();
                    
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
