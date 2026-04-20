using IsotopesStats.Models;
using IsotopesStats.Domain.Interfaces;

namespace IsotopesStats.Services;

public class AuthService
{
    private readonly IAuthRepository _repository;

    public AuthService(IAuthRepository repository)
    {
        _repository = repository;
    }

    public Task<User?> LoginAsync(string email, string password) => _repository.LoginAsync(email, password);
    public Task LogoutAsync() => _repository.LogoutAsync();
    public Task<List<UserRolesSummaryView>> GetUsersAsync() => _repository.GetUsersAsync();
    public Task<bool> IsEmailUniqueAsync(string email, string excludeUserId = "") => _repository.IsEmailUniqueAsync(email, excludeUserId);
    public Task<bool> RegisterAsync(string email, List<int> roleIds) => _repository.RegisterAsync(email, roleIds);
    public Task UpdateUserAsync(User user, List<int> newRoleIds) => _repository.UpdateUserAsync(user, newRoleIds);
    public Task UpdateUserPasswordAsync(string userId, string newPassword) => _repository.UpdateUserPasswordAsync(userId, newPassword);
    public Task<User?> GetUserByIdAsync(string userId) => _repository.GetUserByIdAsync(userId);
    public Task DeleteUserAsync(string userId) => _repository.DeleteUserAsync(userId);
    public Task<List<UserRole>> GetUserRolesAsync(bool onlyActive = false) => _repository.GetUserRolesAsync(onlyActive);
    public Task<List<UserRole>> GetUserRolesForUserAsync(string userId) => _repository.GetUserRolesForUserAsync(userId);
    public Task<List<UserLog>> GetUserLogsAsync(int limit = 100) => _repository.GetUserLogsAsync(limit);
    public Task AddLogAsync(UserLog log) => _repository.AddLogAsync(log);
    public Task<bool> GeneratePasswordResetTokenAsync(string email) => _repository.GeneratePasswordResetTokenAsync(email);
    public Task<bool> ResetPasswordAsync(string email, string token, string newPassword) => _repository.ResetPasswordAsync(email, token, newPassword);
    public Task<List<Permission>> GetPermissionsAsync() => _repository.GetPermissionsAsync();
    public Task<bool> IsRoleNameUniqueAsync(string name, int excludeId = 0) => _repository.IsRoleNameUniqueAsync(name, excludeId);
    public Task AddUserRoleAsync(UserRole role, List<int> permissionIds) => _repository.AddUserRoleAsync(role, permissionIds);
    public Task UpdateUserRoleAsync(UserRole role, List<int> permissionIds) => _repository.UpdateUserRoleAsync(role, permissionIds);
    public Task DeleteUserRoleAsync(int roleId) => _repository.DeleteUserRoleAsync(roleId);
}
