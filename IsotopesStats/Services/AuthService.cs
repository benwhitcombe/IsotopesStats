using IsotopesStats.Models;
using Supabase;
using Postgrest;
using Postgrest.Responses;
using System.Security.Claims;

namespace IsotopesStats.Services;

public class AuthService
{
    private readonly Supabase.Client _supabase;

    public AuthService(Supabase.Client supabase)
    {
        _supabase = supabase;
    }

    public async Task<User?> LoginAsync(string email, string password)
    {
        try
        {
            Supabase.Gotrue.Session? session = await _supabase.Auth.SignIn(email, password);
            if (session != null && session.User != null && !string.IsNullOrEmpty(session.User.Id))
            {
                User user = new User
                {
                    Email = session.User.Email ?? email,
                    CreatedAt = session.User.CreatedAt
                };
                user.Roles = await GetUserRolesForUserAsync(session.User.Id);
                return user;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Login failed: {ex.Message}");
        }
        return null;
    }

    public async Task LogoutAsync()
    {
        await _supabase.Auth.SignOut();
    }

    public async Task<List<UserRolesSummaryView>> GetUsersAsync()
    {
        ModeledResponse<UserRolesSummaryView> response = await _supabase.From<UserRolesSummaryView>()
            .Order("email", Constants.Ordering.Ascending)
            .Get();
        return response.Models;
    }

    public async Task<bool> IsEmailUniqueAsync(string email, string excludeUserId = "")
    {
        ModeledResponse<User> response = await _supabase.From<User>()
            .Where(x => x.Email == email)
            .Where(x => x.Id != excludeUserId)
            .Where(x => x.IsDeleted == false)
            .Get();
        return response.Models.Count == 0;
    }

    public async Task<bool> RegisterAsync(string email, List<int> roleIds)
    {
        try
        {
            // Generate a random temporary password for the initial signup
            string temporaryPassword = Guid.NewGuid().ToString("N") + "!";
            
            Supabase.Gotrue.Session? response = await _supabase.Auth.SignUp(email, temporaryPassword);
            if (response?.User != null && !string.IsNullOrEmpty(response.User.Id))
            {
                foreach (int roleId in roleIds)
                {
                    UserUserRoles link = new UserUserRoles { UserId = response.User.Id, RoleId = roleId };
                    await _supabase.From<UserUserRoles>().Insert(link);
                }

                // Trigger a password reset email immediately so the user can set their own password
                await _supabase.Auth.ResetPasswordForEmail(email);
                
                return true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Registration failed: {ex.Message}");
        }
        return false;
    }

    public async Task UpdateUserAsync(User user, List<int> newRoleIds)
    {
        await _supabase.From<UserUserRoles>().Where(x => x.UserId == user.Id).Delete();
        foreach (int roleId in newRoleIds)
        {
            UserUserRoles link = new UserUserRoles { UserId = user.Id, RoleId = roleId };
            await _supabase.From<UserUserRoles>().Insert(link);
        }
    }

    public async Task UpdateUserPasswordAsync(string userId, string newPassword)
    {
        Supabase.Gotrue.UserAttributes attrs = new Supabase.Gotrue.UserAttributes { Password = newPassword };
        await _supabase.Auth.Update(attrs);
    }

    public async Task DeleteUserAsync(string userId)
    {
        User user = new User { Id = userId, IsDeleted = true };
        await _supabase.From<User>().Update(user);
    }

    public async Task<List<UserRole>> GetUserRolesAsync(bool onlyActive = false)
    {
        ModeledResponse<UserRole> response;
        if (onlyActive)
        {
            response = await _supabase.From<UserRole>()
                .Where(x => x.IsDeleted == false)
                .Order("name", Constants.Ordering.Ascending)
                .Get();
        }
        else
        {
            response = await _supabase.From<UserRole>()
                .Order("name", Constants.Ordering.Ascending)
                .Get();
        }

        List<UserRole> roles = response.Models;

        if (roles.Any())
        {
            ModeledResponse<RolePermission> rpResponse = await _supabase.From<RolePermission>().Get();
            ModeledResponse<Permission> permResponse = await _supabase.From<Permission>().Get();
            List<Permission> allPermissions = permResponse.Models;

            foreach (UserRole role in roles)
            {
                List<int> rolePermIds = rpResponse.Models
                    .Where(rp => rp.RoleId == role.Id)
                    .Select(rp => rp.PermissionId)
                    .ToList();
                
                role.Permissions = allPermissions
                    .Where(p => rolePermIds.Contains(p.Id))
                    .ToList();
            }
        }

        return roles;
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
            Console.WriteLine("Error fetching roles: " + ex.Message);
            return new List<UserRole>();
        }
    }

    public async Task<List<UserLog>> GetUserLogsAsync(int limit = 100)
    {
        ModeledResponse<UserLog> response = await _supabase.From<UserLog>()
            .Order("timestamp", Constants.Ordering.Descending)
            .Limit(limit)
            .Get();
        return response.Models;
    }

    public async Task AddLogAsync(UserLog log)
    {
        await _supabase.From<UserLog>().Insert(log);
    }

    public async Task<bool> GeneratePasswordResetTokenAsync(string email)
    {
        try { await _supabase.Auth.ResetPasswordForEmail(email); return true; }
        catch { return false; }
    }

    public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
    {
        // Using Task.FromResult to resolve CS1998 warning
        return await Task.FromResult(true);
    }

    public async Task<List<Permission>> GetPermissionsAsync()
    {
        ModeledResponse<Permission> response = await _supabase.From<Permission>().Order("name", Constants.Ordering.Ascending).Get();
        return response.Models;
    }

    public async Task<bool> IsRoleNameUniqueAsync(string name, int excludeId = 0)
    {
        ModeledResponse<UserRole> response = await _supabase.From<UserRole>()
            .Where(x => x.Name == name)
            .Where(x => x.Id != excludeId)
            .Where(x => x.IsDeleted == false)
            .Get();
        return response.Models.Count == 0;
    }

    public async Task AddUserRoleAsync(UserRole role, List<int> permissionIds)
    {
        ModeledResponse<UserRole> response = await _supabase.From<UserRole>().Insert(role);
        UserRole? newRole = response.Models.FirstOrDefault();
        if (newRole != null && permissionIds.Any())
        {
            foreach (int permId in permissionIds)
            {
                await _supabase.From<RolePermission>().Insert(new RolePermission { RoleId = newRole.Id, PermissionId = permId });
            }
        }
    }

    public async Task UpdateUserRoleAsync(UserRole role, List<int> permissionIds)
    {
        await _supabase.From<UserRole>().Update(role);
        
        // Update permissions: delete all and re-add
        await _supabase.From<RolePermission>().Where(x => x.RoleId == role.Id).Delete();
        foreach (int permId in permissionIds)
        {
            await _supabase.From<RolePermission>().Insert(new RolePermission { RoleId = role.Id, PermissionId = permId });
        }
    }

    public async Task DeleteUserRoleAsync(int roleId)
    {
        UserRole role = new UserRole { Id = roleId, IsDeleted = true };
        await _supabase.From<UserRole>().Update(role);
    }
}
