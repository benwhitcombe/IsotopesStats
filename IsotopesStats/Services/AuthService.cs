using IsotopesStats.Models;
using Supabase;
using Postgrest;
using System.Security.Claims;

namespace IsotopesStats.Services;

public class AuthService
{
    private readonly Client _supabase;

    public AuthService(Client supabase)
    {
        _supabase = supabase;
    }

    public async Task<User?> LoginAsync(string email, string password)
    {
        try
        {
            var session = await _supabase.Auth.SignIn(email, password);
            if (session != null && session.User != null)
            {
                var user = new User
                {
                    Email = session.User.Email ?? email,
                    CreatedAt = DateTime.Parse(session.User.CreatedAt ?? DateTime.UtcNow.ToString())
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

    public async Task<List<User>> GetUsersAsync()
    {
        // Use the flattened view for the user list
        var response = await _supabase.From<User>("v_user_roles_summary")
            .Order("email", Constants.Ordering.Ascending)
            .Get();
        
        // Note: The view returns comma-separated role names. 
        // For the full User object used in editing, we might need a separate query.
        return response.Models;
    }

    public async Task<bool> IsEmailUniqueAsync(string email, int excludeUserId = 0)
    {
        var response = await _supabase.From<User>()
            .Where(x => x.Email == email)
            .Where(x => x.Id != excludeUserId)
            .Where(x => x.IsDeleted == false)
            .Count(Constants.CountType.Exact);
        
        return response == 0;
    }

    public async Task<bool> RegisterAsync(string email, string password, List<int> roleIds)
    {
        try
        {
            // 1. Create User in Supabase Auth
            var attrs = new Supabase.Gotrue.AdminUserAttributes { Email = email, Password = password };
            var response = await _supabase.Auth.Admin.CreateUser(attrs);
            
            if (response != null)
            {
                // 2. Link Roles in our custom table
                foreach (int roleId in roleIds)
                {
                    var link = new UserUserRoles { UserId = response.Id, RoleId = roleId };
                    await _supabase.From<UserUserRoles>().Insert(link);
                }
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
        // 1. Update Email in Auth (Optional/Complex in Supabase)
        // 2. Update Roles in custom table
        await _supabase.From<UserUserRoles>().Where(x => x.UserId == user.Id.ToString()).Delete();
        foreach (int roleId in newRoleIds)
        {
            var link = new UserUserRoles { UserId = user.Id.ToString(), RoleId = roleId };
            await _supabase.From<UserUserRoles>().Insert(link);
        }
    }

    public async Task UpdateUserPasswordAsync(int userId, string newPassword)
    {
        // In Supabase, password updates are usually done via Auth API
        // This requires the Supabase User ID (string)
    }

    public async Task DeleteUserAsync(int userId)
    {
        var user = new User { Id = userId, IsDeleted = true };
        await _supabase.From<User>().Update(user);
    }

    public async Task<List<UserRole>> GetUserRolesAsync(bool onlyActive = false)
    {
        var query = _supabase.From<UserRole>();
        if (onlyActive) query = query.Where(x => x.IsDeleted == false);
        var response = await query.Order("name", Constants.Ordering.Ascending).Get();
        return response.Models;
    }

    private async Task<List<UserRole>> GetUserRolesForUserAsync(string supabaseUserId)
    {
        var response = await _supabase
            .From<UserRole>()
            .Select("id, name, isdeleted, rolepermissions(permissions(id, name))")
            .Join<UserUserRoles>("id", "roleid")
            .Where<UserUserRoles>(x => x.UserId == supabaseUserId)
            .Get();

        return response.Models;
    }

    public async Task<List<UserLog>> GetUserLogsAsync(int limit = 100)
    {
        var response = await _supabase.From<UserLog>()
            .Order("timestamp", Constants.Ordering.Descending)
            .Limit(limit)
            .Get();
        return response.Models;
    }

    public async Task AddLogAsync(UserLog log)
    {
        await _supabase.From<UserLog>().Insert(log);
    }
}
