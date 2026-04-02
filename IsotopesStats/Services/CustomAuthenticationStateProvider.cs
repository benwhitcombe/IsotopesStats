using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using IsotopesStats.Models;

namespace IsotopesStats.Services;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly ProtectedSessionStorage _sessionStorage;
    private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());

    public CustomAuthenticationStateProvider(ProtectedSessionStorage sessionStorage)
    {
        _sessionStorage = sessionStorage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            ProtectedBrowserStorageResult<User> userSessionStorageResult = await _sessionStorage.GetAsync<User>("UserSession");
            User? userSession = userSessionStorageResult.Success ? userSessionStorageResult.Value : null;

            if (userSession == null)
                return await Task.FromResult(new AuthenticationState(_anonymous));

            return await Task.FromResult(new AuthenticationState(CreateClaimsPrincipal(userSession)));
        }
        catch
        {
            return await Task.FromResult(new AuthenticationState(_anonymous));
        }
    }

    public async Task UpdateAuthenticationState(User? userSession)
    {
        ClaimsPrincipal claimsPrincipal;

        if (userSession != null)
        {
            await _sessionStorage.SetAsync("UserSession", userSession);
            claimsPrincipal = CreateClaimsPrincipal(userSession);
        }
        else
        {
            await _sessionStorage.DeleteAsync("UserSession");
            claimsPrincipal = _anonymous;
        }

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
    }

    private ClaimsPrincipal CreateClaimsPrincipal(User user)
    {
        List<Claim> claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role?.Name ?? "User")
        };

        if (user.Role?.Permissions != null)
        {
            foreach (Permission permission in user.Role.Permissions)
            {
                claims.Add(new Claim("Permission", permission.Name));
            }
        }

        return new ClaimsPrincipal(new ClaimsIdentity(claims, "CustomAuth"));
    }
}
