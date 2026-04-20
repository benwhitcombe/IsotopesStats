using System.Collections.Generic;
using System.Threading.Tasks;
using IsotopesStats.Models;

namespace IsotopesStats.Domain.Interfaces;

public interface IAuthRepository
{
    Task<User?> LoginAsync(string email, string password);
    Task LogoutAsync();
    Task<List<UserRolesSummaryView>> GetUsersAsync();
    Task<bool> IsEmailUniqueAsync(string email, string excludeUserId = "");
    Task<bool> RegisterAsync(string email, List<int> roleIds);
    Task UpdateUserAsync(User user, List<int> newRoleIds);
    Task UpdateUserPasswordAsync(string userId, string newPassword);
    Task<User?> GetUserByIdAsync(string userId);
    Task DeleteUserAsync(string userId);
    Task<List<UserRole>> GetUserRolesAsync(bool onlyActive = false);
    Task<List<UserRole>> GetUserRolesForUserAsync(string userId);
    Task<List<UserLog>> GetUserLogsAsync(int limit = 100);
    Task AddLogAsync(UserLog log);
    Task<bool> GeneratePasswordResetTokenAsync(string email);
    Task<bool> ResetPasswordAsync(string email, string token, string newPassword);
    Task<List<Permission>> GetPermissionsAsync();
    Task<bool> IsRoleNameUniqueAsync(string name, int excludeId = 0);
    Task AddUserRoleAsync(UserRole role, List<int> permissionIds);
    Task UpdateUserRoleAsync(UserRole role, List<int> permissionIds);
    Task DeleteUserRoleAsync(int roleId);
}
