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
            .Filter("isdeleted", Operator.Equals, false);

        if (!string.IsNullOrEmpty(excludeUserId))
        {
            query = query.Filter("id", Operator.NotEqual, excludeUserId);
        }

        ModeledResponse<UserDTO> response = await query.Get();
        return response.Models.Count == 0;
    }

    public async Task<bool> RegisterAsync(string email, List<int> roleIds)
    {
        try
        {
            string temporaryPassword = Guid.NewGuid().ToString("N") + "!";
            Session? response = await Supabase.Auth.SignUp(email, temporaryPassword);
            if (response?.User != null && !string.IsNullOrEmpty(response.User.Id))
            {
                foreach (int roleId in roleIds)
                {
                    UserUserRoles link = new UserUserRoles { UserId = response.User.Id, RoleId = roleId };
                    await Supabase.From<UserUserRolesDTO>().Insert(Mapper.ToDTO(link));
                }
                await Supabase.Auth.ResetPasswordForEmail(email);
                return true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Registration failed: {ex.Message}");
        }
        return false;
    }

    public async Task UpdateUserAsync(DomainUser user, List<int> newRoleIds)
    {
        await Supabase.From<UserUserRolesDTO>().Filter("userid", Operator.Equals, user.Id).Delete();
        foreach (int roleId in newRoleIds)
        {
            UserUserRoles link = new UserUserRoles { UserId = user.Id, RoleId = roleId };
            await Supabase.From<UserUserRolesDTO>().Insert(Mapper.ToDTO(link));
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
        DomainUser user = new DomainUser { Id = userId, IsDeleted = true };
        await Supabase.From<UserDTO>().Update(Mapper.ToDTO(user));
        await Supabase.From<UserUserRolesDTO>().Filter("userid", Operator.Equals, userId).Delete();
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
            await Supabase.Auth.ResetPasswordForEmail(email);
            return true;
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
            await Supabase.Auth.VerifyOTP(email, token, EmailOtpType.Recovery);
            UserAttributes attrs = new UserAttributes { Password = newPassword };
            await Supabase.Auth.Update(attrs);
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
        await Supabase.From<RolePermissionDTO>().Filter("roleid", Operator.Equals, role.Id).Delete();

        foreach (int permId in permissionIds)
        {
            RolePermission link = new RolePermission { RoleId = role.Id, PermissionId = permId };
            await Supabase.From<RolePermissionDTO>().Insert(Mapper.ToDTO(link));
        }
    }

    public Task DeleteUserRoleAsync(int roleId) => 
        SoftDeleteAsync<UserRoleDTO>(roleId);
}
