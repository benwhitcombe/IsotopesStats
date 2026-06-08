using IsotopesStats.Domain.Models;
using IsotopesStats.SupabaseRepository.Models;
using IsotopesStats.SupabaseRepository.Mappings;
using Supabase;
using Postgrest;
using Postgrest.Responses;
using static Postgrest.Constants;
using System.Security.Claims;
using IsotopesStats.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Supabase.Gotrue;
using static Supabase.Gotrue.Constants;
using DomainUser = IsotopesStats.Domain.Models.User;

namespace IsotopesStats.SupabaseRepository.Repositories;

internal class AuthRepository : BaseRepository, IAuthRepository
{
    public AuthRepository(Supabase.Client supabase) : base(supabase, new SupabaseMapper())
    {
    }

    public async Task<DomainUser?> LoginAsync(string email, string password)
    {
        try
        {
            Session? session = await Supabase.Auth.SignIn(email, password);
            if (session != null && session.User != null && !string.IsNullOrEmpty(session.User.Id))
            {
                return new DomainUser
                {
                    Id = session.User.Id,
                    Email = session.User.Email ?? email,
                    CreatedAt = session.User.CreatedAt
                };
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
        await Supabase.Auth.SignOut();
    }

    public Task<List<UserRolesSummaryView>> GetUsersAsync() => 
        GetListAsync<UserRolesSummaryView, UserRolesSummaryViewDTO>(Mapper.ToModel, "email");

    public async Task<bool> IsEmailUniqueAsync(string email, string excludeUserId = "")
    {
        Table<UserDTO> query = Supabase.From<UserDTO>()
            .Filter("email", Operator.Equals, email)
            .Filter("isdeleted", Operator.Equals, "false");

        if (!string.IsNullOrEmpty(excludeUserId))
        {
            query = query.Filter("id", Operator.NotEqual, excludeUserId);
        }

        ModeledResponse<UserDTO> response = await query.Get();
        return response.Models.Count == 0;
    }

    public async Task<bool> RegisterAsync(string email, List<int> roleIds)
    {
        var body = new Dictionary<string, object>
        {
            { "email", email },
            { "roleIds", roleIds }
        };

            var options = new Supabase.Functions.Client.InvokeFunctionOptions { Body = body };
            var token = Supabase.Auth.CurrentSession?.AccessToken;
            var response = await Supabase.Functions.Invoke("create-user", token, options);
            
            // Supabase Functions returns HTTP OK if the function executed successfully
            if (!string.IsNullOrEmpty(response))
            {
                return true;
            }
            return false;
    }

    public async Task<bool> ResendWelcomeEmailAsync(string email)
    {
        var body = new Dictionary<string, object>
        {
            { "email", email },
            { "action", "resend" }
        };

        var options = new Supabase.Functions.Client.InvokeFunctionOptions { Body = body };
        var token = Supabase.Auth.CurrentSession?.AccessToken;
        var response = await Supabase.Functions.Invoke("create-user", token, options);
        
        if (!string.IsNullOrEmpty(response))
        {
            return true;
        }
        return false;
    }

    public async Task UpdateUserAsync(DomainUser user, List<int> newRoleIds)
    {
        var existingResponse = await Supabase.From<UserUserRolesDTO>().Filter("userid", Operator.Equals, user.Id).Get();
        var existingIds = existingResponse.Models.Select(x => x.RoleId).ToList();

        var toDelete = existingIds.Except(newRoleIds).ToList();
        var toInsert = newRoleIds.Except(existingIds).ToList();

        // Insert new roles first
        foreach (int roleId in toInsert)
        {
            UserUserRoles link = new UserUserRoles { UserId = user.Id, RoleId = roleId };
            await Supabase.From<UserUserRolesDTO>().Insert(Mapper.ToDTO(link));
        }

        // Delete removed roles last
        if (toDelete.Any())
        {
            await Supabase.From<UserUserRolesDTO>()
                .Filter("userid", Operator.Equals, user.Id)
                .Filter("roleid", Operator.In, toDelete)
                .Delete();
        }
        
        await UpdateAsync(user, Mapper.ToDTO);
    }

    public async Task UpdateUserPasswordAsync(string userId, string newPassword)
    {
        UserAttributes attrs = new UserAttributes { Password = newPassword };
        await Supabase.Auth.Update(attrs);
    }

    public async Task<DomainUser?> GetUserByIdAsync(string userId)
    {
        if (string.IsNullOrEmpty(userId)) return null;
        try
        {
            ModeledResponse<UserDTO> response = await Supabase.From<UserDTO>().Filter("id", Operator.Equals, userId).Get();
            UserDTO? dto = response.Models.FirstOrDefault();
            return dto != null ? Mapper.ToModel(dto) : null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"GetUserByIdAsync Exception: {ex.Message}");
            return null;
        }
    }

    public async Task DeleteUserAsync(string userId)
    {
        await Supabase.From<UserUserRolesDTO>().Filter("userid", Operator.Equals, userId).Delete();
        await Supabase.From<UserDTO>().Filter("id", Operator.Equals, userId).Delete();
    }

    public async Task<List<UserRole>> GetUserRolesAsync(bool onlyActive = false)
    {
        List<UserRole> roles = await GetListAsync<UserRole, UserRoleDTO>(Mapper.ToModel, "name", ordering: Ordering.Ascending, onlyActive: onlyActive);
        
        foreach (UserRole role in roles)
        {
            await PopulateRolePermissions(role);
        }

        return roles;
    }

    public async Task<List<UserRole>> GetUserRolesForUserAsync(string userId)
    {
        ModeledResponse<UserUserRolesDTO> urResponse = await Supabase.From<UserUserRolesDTO>()
            .Filter("userid", Operator.Equals, userId)
            .Get();

        if (!urResponse.Models.Any()) return new List<UserRole>();

        List<int> roleIds = urResponse.Models.Select(x => x.RoleId).ToList();
        ModeledResponse<UserRoleDTO> rolesResponse = await Supabase.From<UserRoleDTO>()
            .Filter("id", Operator.In, roleIds)
            .Get();

        List<UserRole> roles = rolesResponse.Models.Select(x => Mapper.ToModel(x)).ToList();
        
        foreach (UserRole role in roles)
        {
            await PopulateRolePermissions(role);
        }

        return roles;
    }

    private async Task PopulateRolePermissions(UserRole role)
    {
        ModeledResponse<RolePermissionDTO> response = await Supabase.From<RolePermissionDTO>()
            .Filter("roleid", Operator.Equals, role.Id)
            .Get();

        role.Permissions = response.Models
            .Where(rp => rp.Permission != null)
            .Select(rp => Mapper.ToModel(rp.Permission!))
            .ToList();
    }

    public async Task<List<UserLog>> GetUserLogsAsync(int limit = 100)
    {
        ModeledResponse<UserLogDTO> response = await Supabase.From<UserLogDTO>()
            .Order("timestamp", Ordering.Descending)
            .Limit(limit)
            .Get();
        return response.Models.Select(x => Mapper.ToModel(x)).ToList();
    }

    public Task AddLogAsync(UserLog log) => 
        InsertAsync(log, Mapper.ToDTO);

    public async Task<bool> GeneratePasswordResetTokenAsync(string email)
    {
        try
        {
            var body = new Dictionary<string, object>
            {
                { "email", email },
                { "action", "forgot_password" }
            };

            var options = new Supabase.Functions.Client.InvokeFunctionOptions { Body = body };
            var response = await Supabase.Functions.Invoke("create-user", null, options);
            
            if (!string.IsNullOrEmpty(response))
            {
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Reset password email failed: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
    {
        try
        {
            await Supabase.Auth.VerifyOTP(email, token, global::Supabase.Gotrue.Constants.EmailOtpType.Recovery);
            UserAttributes attrs = new UserAttributes { Password = newPassword };
            await Supabase.Auth.Update(attrs);
            
            // Force the user to log in manually using their new password
            await Supabase.Auth.SignOut();
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Reset password failed: {ex.Message}");
            return false;
        }
    }

    public Task<List<Permission>> GetPermissionsAsync() => 
        GetListAsync<Permission, PermissionDTO>(Mapper.ToModel, "name", onlyActive: false);

    public Task<bool> IsRoleNameUniqueAsync(string name, int excludeId = 0) => 
        IsUniqueAsync<UserRoleDTO>("name", name, excludeId);

    public async Task AddUserRoleAsync(UserRole role, List<int> permissionIds)
    {
        int roleId = await InsertAsync(role, Mapper.ToDTO);

        if (roleId != 0)
        {
            foreach (int permId in permissionIds)
            {
                RolePermission link = new RolePermission { RoleId = roleId, PermissionId = permId };
                await Supabase.From<RolePermissionDTO>().Insert(Mapper.ToDTO(link));
            }
        }
    }

    public async Task UpdateUserRoleAsync(UserRole role, List<int> permissionIds)
    {
        await UpdateAsync(role, Mapper.ToDTO);
        
        var existingResponse = await Supabase.From<RolePermissionDTO>().Filter("roleid", Operator.Equals, role.Id).Get();
        var existingIds = existingResponse.Models.Select(x => x.PermissionId).ToList();

        var toDelete = existingIds.Except(permissionIds).ToList();
        var toInsert = permissionIds.Except(existingIds).ToList();

        // Insert new permissions first (while user still has Manage Roles)
        foreach (int permId in toInsert)
        {
            RolePermission link = new RolePermission { RoleId = role.Id, PermissionId = permId };
            await Supabase.From<RolePermissionDTO>().Insert(Mapper.ToDTO(link));
        }

        // Delete removed permissions last
        if (toDelete.Any())
        {
            await Supabase.From<RolePermissionDTO>()
                .Filter("roleid", Operator.Equals, role.Id)
                .Filter("permissionid", Operator.In, toDelete)
                .Delete();
        }
    }

    public Task DeleteUserRoleAsync(int roleId) => 
        SoftDeleteAsync<UserRoleDTO>(roleId);
}
