using IsotopesStats.Models;
using SupabaseRepository.Models;
using SupabaseRepository.Mappings;
using Supabase;
using Postgrest;
using Postgrest.Responses;
using System.Security.Claims;
using IsotopesStats.Domain.Interfaces;

namespace SupabaseRepository.Repositories;

public class SupabaseAuthRepository : IAuthRepository
{
    private readonly Supabase.Client _supabase;

    public SupabaseAuthRepository(Supabase.Client supabase)
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
                    Id = session.User.Id,
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
        ModeledResponse<UserRolesSummaryViewDto> response = await _supabase.From<UserRolesSummaryViewDto>()
            .Order("email", Postgrest.Constants.Ordering.Ascending)
            .Get();
        return response.Models.Select(x => x.ToModel()).ToList();
    }

    public async Task<bool> IsEmailUniqueAsync(string email, string excludeUserId = "")
    {
        ModeledResponse<UserDto> response = await _supabase.From<UserDto>()
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
                    await _supabase.From<UserUserRolesDto>().Insert(link.ToDto());
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
        await _supabase.From<UserUserRolesDto>().Where(x => x.UserId == user.Id).Delete();
        foreach (int roleId in newRoleIds)
        {
            UserUserRoles link = new UserUserRoles { UserId = user.Id, RoleId = roleId };
            await _supabase.From<UserUserRolesDto>().Insert(link.ToDto());
        }
    }

    public async Task UpdateUserPasswordAsync(string userId, string newPassword)
    {
        Supabase.Gotrue.UserAttributes attrs = new Supabase.Gotrue.UserAttributes { Password = newPassword };
        await _supabase.Auth.Update(attrs);
    }

    public async Task<User?> GetUserByIdAsync(string userId)
    {
        if (string.IsNullOrEmpty(userId)) return null;
        ModeledResponse<UserDto> response = await _supabase.From<UserDto>().Where(x => x.Id == userId).Get();
        return response.Models.FirstOrDefault()?.ToModel();
    }

    public async Task DeleteUserAsync(string userId)
    {
        // 1. Mark as deleted in users table
        User user = new User { Id = userId, IsDeleted = true };
        await _supabase.From<UserDto>().Update(user.ToDto());

        // 2. Revoke all roles immediately so they lose all permissions
        await _supabase.From<UserUserRolesDto>().Where(x => x.UserId == userId).Delete();
    }

    public async Task<List<UserRole>> GetUserRolesAsync(bool onlyActive = false)
    {
        ModeledResponse<UserRoleDto> response;
        if (onlyActive)
        {
            response = await _supabase.From<UserRoleDto>()
                .Where(x => x.IsDeleted == false)
                .Order("name", Postgrest.Constants.Ordering.Ascending)
                .Get();
        }
        else
        {
            response = await _supabase.From<UserRoleDto>()
                .Order("name", Postgrest.Constants.Ordering.Ascending)
                .Get();
        }

        List<UserRole> roles = response.Models.Select(x => x.ToModel()).ToList();

        if (roles.Any())
        {
            ModeledResponse<RolePermissionDto> rpResponse = await _supabase.From<RolePermissionDto>().Get();
            ModeledResponse<PermissionDto> permResponse = await _supabase.From<PermissionDto>().Get();
            List<Permission> allPermissions = permResponse.Models.Select(x => x.ToModel()).ToList();

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

    public async Task<List<UserRole>> GetUserRolesForUserAsync(string supabaseUserId)
    {
        if (string.IsNullOrEmpty(supabaseUserId)) return new List<UserRole>();

        try 
        {
            ModeledResponse<UserUserRolesDto> urResponse = await _supabase.From<UserUserRolesDto>().Where(x => x.UserId == supabaseUserId).Get();
            List<int> roleIds = urResponse.Models.Select(x => x.RoleId).ToList();

            if (!roleIds.Any()) return new List<UserRole>();

            ModeledResponse<UserRoleDto> rolesResponse = await _supabase.From<UserRoleDto>().Get();
            List<UserRole> roles = rolesResponse.Models.Where(r => roleIds.Contains(r.Id)).Select(x => x.ToModel()).ToList();

            ModeledResponse<RolePermissionDto> rpResponse = await _supabase.From<RolePermissionDto>().Get();
            List<int> permissionIdsForTheseRoles = rpResponse.Models
                .Where(rp => roleIds.Contains(rp.RoleId))
                .Select(x => x.PermissionId)
                .Distinct()
                .ToList();

            if (permissionIdsForTheseRoles.Any())
            {
                ModeledResponse<PermissionDto> permResponse = await _supabase.From<PermissionDto>().Get();
                List<Permission> perms = permResponse.Models.Select(x => x.ToModel()).ToList();

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
        ModeledResponse<UserLogDto> response = await _supabase.From<UserLogDto>()
            .Order("timestamp", Postgrest.Constants.Ordering.Descending)
            .Limit(limit)
            .Get();
        return response.Models.Select(x => x.ToModel()).ToList();
    }

    public async Task AddLogAsync(UserLog log)
    {
        await _supabase.From<UserLogDto>().Insert(log.ToDto());
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
        ModeledResponse<PermissionDto> response = await _supabase.From<PermissionDto>().Order("name", Postgrest.Constants.Ordering.Ascending).Get();
        return response.Models.Select(x => x.ToModel()).ToList();
    }

    public async Task<bool> IsRoleNameUniqueAsync(string name, int excludeId = 0)
    {
        ModeledResponse<UserRoleDto> response = await _supabase.From<UserRoleDto>()
            .Where(x => x.Name == name)
            .Where(x => x.Id != excludeId)
            .Where(x => x.IsDeleted == false)
            .Get();
        return response.Models.Count == 0;
    }

    public async Task AddUserRoleAsync(UserRole role, List<int> permissionIds)
    {
        ModeledResponse<UserRoleDto> response = await _supabase.From<UserRoleDto>().Insert(role.ToDto());
        UserRoleDto? newRole = response.Models.FirstOrDefault();
        if (newRole != null && permissionIds.Any())
        {
            foreach (int permId in permissionIds)
            {
                await _supabase.From<RolePermissionDto>().Insert(new RolePermission { RoleId = newRole.Id, PermissionId = permId }.ToDto());
            }
        }
    }

    public async Task UpdateUserRoleAsync(UserRole role, List<int> permissionIds)
    {
        await _supabase.From<UserRoleDto>().Update(role.ToDto());
        
        // Update permissions: delete all and re-add
        await _supabase.From<RolePermissionDto>().Where(x => x.RoleId == role.Id).Delete();
        foreach (int permId in permissionIds)
        {
            await _supabase.From<RolePermissionDto>().Insert(new RolePermission { RoleId = role.Id, PermissionId = permId }.ToDto());
        }
    }

    public async Task DeleteUserRoleAsync(int roleId)
    {
        UserRole role = new UserRole { Id = roleId, IsDeleted = true };
        await _supabase.From<UserRoleDto>().Update(role.ToDto());
    }
}
