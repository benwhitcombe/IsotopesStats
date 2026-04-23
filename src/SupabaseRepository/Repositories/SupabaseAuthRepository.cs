using IsotopesStats.Domain.Models;
using IsotopesStats.SupabaseRepository.Models;
using IsotopesStats.SupabaseRepository.Mappings;
using Supabase;
using Postgrest;
using Postgrest.Responses;
using static Postgrest.Constants;
using System.Security.Claims;
using IsotopesStats.Domain.Interfaces;

namespace IsotopesStats.SupabaseRepository.Repositories;

internal class SupabaseAuthRepository : IAuthRepository
{
    private readonly Supabase.Client _supabase;
    private readonly SupabaseMapper _mapper = new();

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
                return new User
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
        await _supabase.Auth.SignOut();
    }

    public async Task<List<UserRolesSummaryView>> GetUsersAsync()
    {
        ModeledResponse<UserRolesSummaryViewDTO> response = await _supabase.From<UserRolesSummaryViewDTO>()
            .Order("email", Postgrest.Constants.Ordering.Ascending)
            .Get();
        return response.Models.Select(x => _mapper.ToModel(x)).ToList();
    }

    public async Task<bool> IsEmailUniqueAsync(string email, string excludeUserId = "")
    {
        Postgrest.Table<UserDTO> query = _supabase.From<UserDTO>()
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
            Supabase.Gotrue.Session? response = await _supabase.Auth.SignUp(email, temporaryPassword);
            if (response?.User != null && !string.IsNullOrEmpty(response.User.Id))
            {
                foreach (int roleId in roleIds)
                {
                    UserUserRoles link = new UserUserRoles { UserId = response.User.Id, RoleId = roleId };
                    await _supabase.From<UserUserRolesDTO>().Insert(_mapper.ToDTO(link));
                }
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
        await _supabase.From<UserUserRolesDTO>().Filter("userid", Operator.Equals, user.Id).Delete();
        foreach (int roleId in newRoleIds)
        {
            UserUserRoles link = new UserUserRoles { UserId = user.Id, RoleId = roleId };
            await _supabase.From<UserUserRolesDTO>().Insert(_mapper.ToDTO(link));
        }
        
        await _supabase.From<UserDTO>().Update(_mapper.ToDTO(user));
    }

    public async Task UpdateUserPasswordAsync(string userId, string newPassword)
    {
        Supabase.Gotrue.UserAttributes attrs = new Supabase.Gotrue.UserAttributes { Password = newPassword };
        await _supabase.Auth.Update(attrs);
    }

    public async Task<User?> GetUserByIdAsync(string userId)
    {
        if (string.IsNullOrEmpty(userId)) return null;
        try
        {
            ModeledResponse<UserDTO> response = await _supabase.From<UserDTO>().Filter("id", Operator.Equals, userId).Get();
            UserDTO? dto = response.Models.FirstOrDefault();
            return dto != null ? _mapper.ToModel(dto) : null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"GetUserByIdAsync Exception: {ex.Message}");
            return null;
        }
    }

    public async Task DeleteUserAsync(string userId)
    {
        User user = new User { Id = userId, IsDeleted = true };
        await _supabase.From<UserDTO>().Update(_mapper.ToDTO(user));
        await _supabase.From<UserUserRolesDTO>().Filter("userid", Operator.Equals, userId).Delete();
    }

    public async Task<List<UserRole>> GetUserRolesAsync(bool onlyActive = false)
    {
        ModeledResponse<UserRoleDTO> response;
        if (onlyActive)
        {
            response = await _supabase.From<UserRoleDTO>().Where(x => x.IsDeleted == false).Order("name", Ordering.Ascending).Get();
        }
        else
        {
            response = await _supabase.From<UserRoleDTO>().Order("name", Ordering.Ascending).Get();
        }
        return response.Models.Select(x => _mapper.ToModel(x)).ToList();
    }

    public async Task<List<UserRole>> GetUserRolesForUserAsync(string userId)
    {
        ModeledResponse<UserUserRolesDTO> urResponse = await _supabase.From<UserUserRolesDTO>()
            .Filter("userid", Operator.Equals, userId)
            .Get();

        if (!urResponse.Models.Any()) return new List<UserRole>();

        List<int> roleIds = urResponse.Models.Select(x => x.RoleId).ToList();

        ModeledResponse<UserRoleDTO> rolesResponse = await _supabase.From<UserRoleDTO>()
            .Filter("id", Operator.In, roleIds)
            .Get();
        return rolesResponse.Models.Select(x => _mapper.ToModel(x)).ToList();
    }

    public async Task<List<UserLog>> GetUserLogsAsync(int limit = 100)
    {
        ModeledResponse<UserLogDTO> response = await _supabase.From<UserLogDTO>()
            .Order("timestamp", Ordering.Descending)
            .Limit(limit)
            .Get();
        return response.Models.Select(x => _mapper.ToModel(x)).ToList();
    }

    public async Task AddLogAsync(UserLog log)
    {
        await _supabase.From<UserLogDTO>().Insert(_mapper.ToDTO(log));
    }

    public async Task<bool> GeneratePasswordResetTokenAsync(string email)
    {
        try
        {
            await _supabase.Auth.ResetPasswordForEmail(email);
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
            await _supabase.Auth.VerifyOTP(email, token, Supabase.Gotrue.Constants.EmailOtpType.Recovery);
            Supabase.Gotrue.UserAttributes attrs = new Supabase.Gotrue.UserAttributes { Password = newPassword };
            await _supabase.Auth.Update(attrs);
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Reset password failed: {ex.Message}");
            return false;
        }
    }

    public async Task<List<Permission>> GetPermissionsAsync()
    {
        ModeledResponse<PermissionDTO> response = await _supabase.From<PermissionDTO>()
            .Order("name", Ordering.Ascending)
            .Get();
        return response.Models.Select(x => _mapper.ToModel(x)).ToList();
    }

    public async Task<bool> IsRoleNameUniqueAsync(string name, int excludeId = 0)
    {
        ModeledResponse<UserRoleDTO> response = await _supabase.From<UserRoleDTO>()
            .Where(x => x.Name == name)
            .Where(x => x.Id != excludeId)
            .Where(x => x.IsDeleted == false)
            .Get();
        return response.Models.Count == 0;
    }

    public async Task AddUserRoleAsync(UserRole role, List<int> permissionIds)
    {
        ModeledResponse<UserRoleDTO> response = await _supabase.From<UserRoleDTO>().Insert(_mapper.ToDTO(role));
        UserRoleDTO? newRole = response.Models.FirstOrDefault();

        if (newRole != null)
        {
            foreach (int permId in permissionIds)
            {
                RolePermission link = new RolePermission { RoleId = newRole.Id, PermissionId = permId };
                await _supabase.From<RolePermissionDTO>().Insert(_mapper.ToDTO(link));
            }
        }
    }

    public async Task UpdateUserRoleAsync(UserRole role, List<int> permissionIds)
    {
        await _supabase.From<UserRoleDTO>().Update(_mapper.ToDTO(role));
        await _supabase.From<RolePermissionDTO>().Filter("roleid", Operator.Equals, role.Id).Delete();

        foreach (int permId in permissionIds)
        {
            RolePermission link = new RolePermission { RoleId = role.Id, PermissionId = permId };
            await _supabase.From<RolePermissionDTO>().Insert(_mapper.ToDTO(link));
        }
    }

    public async Task DeleteUserRoleAsync(int roleId)
    {
        UserRole role = new UserRole { Id = roleId, IsDeleted = true };
        await _supabase.From<UserRoleDTO>().Update(_mapper.ToDTO(role));
    }
}

