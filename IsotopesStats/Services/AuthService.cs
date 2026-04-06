using Microsoft.Data.Sqlite;
using IsotopesStats.Models;
using BCrypt.Net;

namespace IsotopesStats.Services;

public class AuthService
{
    private const string ConnectionString = "Data Source=Data/IsotopesStats.db";

    public async Task<User?> LoginAsync(string email, string password)
    {
        using (SqliteConnection connection = new SqliteConnection(ConnectionString))
        {
            await connection.OpenAsync();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = 
            @"
                SELECT u.Id, u.Email, u.PasswordHash, u.RoleId, u.CreatedAt, u.IsDeleted, r.Name, r.IsDeleted
                FROM Users u
                LEFT JOIN UserRoles r ON u.RoleId = r.Id
                WHERE u.Email = $email AND u.IsDeleted = 0
            ";
            command.Parameters.AddWithValue("$email", email);

            using (SqliteDataReader reader = await command.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    string storedHash = reader.GetString(2);
                    if (BCrypt.Net.BCrypt.Verify(password, storedHash))
                    {
                        int roleId = reader.GetInt32(3);
                        User user = new User
                        {
                            Id = reader.GetInt32(0),
                            Email = reader.GetString(1),
                            PasswordHash = storedHash,
                            RoleId = roleId,
                            CreatedAt = DateTime.Parse(reader.GetString(4)),
                            IsDeleted = reader.GetInt32(5) == 1,
                            Role = new UserRole
                            {
                                Id = roleId,
                                Name = reader.IsDBNull(6) ? "Unknown" : reader.GetString(6),
                                IsDeleted = reader.IsDBNull(7) || reader.GetInt32(7) == 1
                            }
                        };

                        // Fetch Permissions
                        user.Role.Permissions = await GetRolePermissionsAsync(roleId);
                        return user;
                    }
                }
            }
        }
        return null;
    }

    private async Task<List<Permission>> GetRolePermissionsAsync(int roleId)
    {
        List<Permission> permissions = new List<Permission>();
        using (SqliteConnection connection = new SqliteConnection(ConnectionString))
        {
            await connection.OpenAsync();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = 
            @"
                SELECT p.Id, p.Name
                FROM Permissions p
                JOIN RolePermissions rp ON p.Id = rp.PermissionId
                WHERE rp.RoleId = $roleId
            ";
            command.Parameters.AddWithValue("$roleId", roleId);

            using (SqliteDataReader reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    permissions.Add(new Permission
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1)
                    });
                }
            }
        }
        return permissions;
    }

    public async Task<bool> RegisterAsync(string email, string password, int roleId)
    {
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
        using (SqliteConnection connection = new SqliteConnection(ConnectionString))
        {
            await connection.OpenAsync();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = 
            @"
                INSERT INTO Users (Email, PasswordHash, RoleId, CreatedAt, IsDeleted)
                VALUES ($email, $hash, $roleId, $createdAt, 0)
            ";
            command.Parameters.AddWithValue("$email", email);
            command.Parameters.AddWithValue("$hash", passwordHash);
            command.Parameters.AddWithValue("$roleId", roleId);
            command.Parameters.AddWithValue("$createdAt", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

            try 
            {
                await command.ExecuteNonQueryAsync();
                return true;
            }
            catch (SqliteException ex) when (ex.SqliteErrorCode == 19) // Unique constraint
            {
                return false;
            }
        }
    }

    public async Task UpdateUserAsync(User user)
    {
        using (SqliteConnection connection = new SqliteConnection(ConnectionString))
        {
            await connection.OpenAsync();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = "UPDATE Users SET Email = $email, RoleId = $roleId WHERE Id = $id";
            command.Parameters.AddWithValue("$email", user.Email);
            command.Parameters.AddWithValue("$roleId", user.RoleId);
            command.Parameters.AddWithValue("$id", user.Id);
            await command.ExecuteNonQueryAsync();
        }
    }

    public async Task UpdateUserPasswordAsync(int userId, string newPassword)
    {
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        using (SqliteConnection connection = new SqliteConnection(ConnectionString))
        {
            await connection.OpenAsync();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = "UPDATE Users SET PasswordHash = $hash WHERE Id = $id";
            command.Parameters.AddWithValue("$hash", passwordHash);
            command.Parameters.AddWithValue("$id", userId);
            await command.ExecuteNonQueryAsync();
        }
    }

    public async Task<List<Permission>> GetPermissionsAsync()
    {
        List<Permission> permissions = new List<Permission>();
        using (SqliteConnection connection = new SqliteConnection(ConnectionString))
        {
            await connection.OpenAsync();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Name FROM Permissions ORDER BY Name";

            using (SqliteDataReader reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    permissions.Add(new Permission
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1)
                    });
                }
            }
        }
        return permissions;
    }

    public async Task<List<UserRole>> GetUserRolesAsync(bool onlyActive = false)
    {
        List<UserRole> roles = new List<UserRole>();
        using (SqliteConnection connection = new SqliteConnection(ConnectionString))
        {
            await connection.OpenAsync();
            SqliteCommand command = connection.CreateCommand();
            // Invert logic for SQL: onlyActive means IsDeleted = 0
            command.CommandText = "SELECT Id, Name, IsDeleted FROM UserRoles " + (onlyActive ? "WHERE IsDeleted = 0 " : "WHERE IsDeleted = 0") + " ORDER BY Name";

            using (SqliteDataReader reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    UserRole role = new UserRole
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        IsDeleted = reader.GetInt32(2) == 1
                    };
                    roles.Add(role);
                }
            }
        }

        foreach (UserRole role in roles)
        {
            role.Permissions = await GetRolePermissionsAsync(role.Id);
        }

        return roles;
    }

    public async Task AddUserRoleAsync(UserRole role)
    {
        using (SqliteConnection connection = new SqliteConnection(ConnectionString))
        {
            await connection.OpenAsync();
            using SqliteTransaction transaction = connection.BeginTransaction();
            try
            {
                SqliteCommand cmd = connection.CreateCommand();
                cmd.Transaction = transaction;
                cmd.CommandText = "INSERT INTO UserRoles (Name, IsDeleted) VALUES ($name, 0); SELECT last_insert_rowid();";
                cmd.Parameters.AddWithValue("$name", role.Name);
                int roleId = Convert.ToInt32(await cmd.ExecuteScalarAsync());

                foreach (Permission p in role.Permissions)
                {
                    SqliteCommand pCmd = connection.CreateCommand();
                    pCmd.Transaction = transaction;
                    pCmd.CommandText = "INSERT INTO RolePermissions (RoleId, PermissionId) VALUES ($roleId, $pId)";
                    pCmd.Parameters.AddWithValue("$roleId", roleId);
                    pCmd.Parameters.AddWithValue("$pId", p.Id);
                    await pCmd.ExecuteNonQueryAsync();
                }
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }

    public async Task UpdateUserRoleAsync(UserRole role)
    {
        using (SqliteConnection connection = new SqliteConnection(ConnectionString))
        {
            await connection.OpenAsync();
            using SqliteTransaction transaction = connection.BeginTransaction();
            try
            {
                SqliteCommand cmd = connection.CreateCommand();
                cmd.Transaction = transaction;
                cmd.CommandText = "UPDATE UserRoles SET Name = $name WHERE Id = $id";
                cmd.Parameters.AddWithValue("$name", role.Name);
                cmd.Parameters.AddWithValue("$id", role.Id);
                await cmd.ExecuteNonQueryAsync();

                SqliteCommand delCmd = connection.CreateCommand();
                delCmd.Transaction = transaction;
                delCmd.CommandText = "DELETE FROM RolePermissions WHERE RoleId = $roleId";
                delCmd.Parameters.AddWithValue("$roleId", role.Id);
                await delCmd.ExecuteNonQueryAsync();

                foreach (Permission p in role.Permissions)
                {
                    SqliteCommand pCmd = connection.CreateCommand();
                    pCmd.Transaction = transaction;
                    pCmd.CommandText = "INSERT INTO RolePermissions (RoleId, PermissionId) VALUES ($roleId, $pId)";
                    pCmd.Parameters.AddWithValue("$roleId", role.Id);
                    pCmd.Parameters.AddWithValue("$pId", p.Id);
                    await pCmd.ExecuteNonQueryAsync();
                }
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }

    public async Task DeleteUserRoleAsync(int roleId)
    {
        using (SqliteConnection connection = new SqliteConnection(ConnectionString))
        {
            await connection.OpenAsync();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = "UPDATE UserRoles SET IsDeleted = 1 WHERE Id = $id";
            command.Parameters.AddWithValue("$id", roleId);
            await command.ExecuteNonQueryAsync();
        }
    }

    public async Task<List<User>> GetUsersAsync()
    {
        List<User> users = new List<User>();
        using (SqliteConnection connection = new SqliteConnection(ConnectionString))
        {
            await connection.OpenAsync();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = 
            @"
                SELECT u.Id, u.Email, u.RoleId, u.CreatedAt, u.IsDeleted, r.Name
                FROM Users u
                LEFT JOIN UserRoles r ON u.RoleId = r.Id
                WHERE u.IsDeleted = 0
                ORDER BY u.Email
            ";

            using (SqliteDataReader reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    users.Add(new User
                    {
                        Id = reader.GetInt32(0),
                        Email = reader.GetString(1),
                        RoleId = reader.GetInt32(2),
                        CreatedAt = DateTime.Parse(reader.GetString(3)),
                        IsDeleted = reader.GetInt32(4) == 1,
                        Role = new UserRole
                        {
                            Id = reader.GetInt32(2),
                            Name = reader.IsDBNull(5) ? "Unknown" : reader.GetString(5)
                        }
                    });
                }
            }
        }
        return users;
    }

    public async Task DeleteUserAsync(int userId)
    {
        using (SqliteConnection connection = new SqliteConnection(ConnectionString))
        {
            await connection.OpenAsync();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = "UPDATE Users SET IsDeleted = 1 WHERE Id = $id";
            command.Parameters.AddWithValue("$id", userId);
            await command.ExecuteNonQueryAsync();
        }
    }

    public async Task<string> GeneratePasswordResetTokenAsync(string email)
    {
        using (SqliteConnection connection = new SqliteConnection(ConnectionString))
        {
            await connection.OpenAsync();
            
            // Check if user exists and is not deleted
            SqliteCommand userCmd = connection.CreateCommand();
            userCmd.CommandText = "SELECT Id FROM Users WHERE Email = $email AND IsDeleted = 0";
            userCmd.Parameters.AddWithValue("$email", email);
            object? userId = await userCmd.ExecuteScalarAsync();
            
            if (userId == null) return string.Empty;

            string token = Guid.NewGuid().ToString();
            SqliteCommand tokenCmd = connection.CreateCommand();
            tokenCmd.CommandText = "INSERT INTO PasswordResetTokens (UserId, Token, ExpiryDate) VALUES ($userId, $token, $expiry)";
            tokenCmd.Parameters.AddWithValue("$userId", userId);
            tokenCmd.Parameters.AddWithValue("$token", token);
            tokenCmd.Parameters.AddWithValue("$expiry", DateTime.UtcNow.AddHours(24).ToString("yyyy-MM-dd HH:mm:ss"));
            
            await tokenCmd.ExecuteNonQueryAsync();
            return token;
        }
    }

    public async Task<bool> ResetPasswordAsync(string token, string newPassword)
    {
        using (SqliteConnection connection = new SqliteConnection(ConnectionString))
        {
            await connection.OpenAsync();
            
            SqliteCommand tokenCmd = connection.CreateCommand();
            tokenCmd.CommandText = 
            @"
                SELECT UserId, ExpiryDate FROM PasswordResetTokens 
                WHERE Token = $token ORDER BY Id DESC LIMIT 1
            ";
            tokenCmd.Parameters.AddWithValue("$token", token);
            
            using (SqliteDataReader reader = await tokenCmd.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    int userId = reader.GetInt32(0);
                    DateTime expiry = DateTime.Parse(reader.GetString(1));
                    
                    if (expiry > DateTime.UtcNow)
                    {
                        string newHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
                        SqliteCommand updateCmd = connection.CreateCommand();
                        updateCmd.CommandText = "UPDATE Users SET PasswordHash = $hash WHERE Id = $userId";
                        updateCmd.Parameters.AddWithValue("$hash", newHash);
                        updateCmd.Parameters.AddWithValue("$userId", userId);
                        
                        await updateCmd.ExecuteNonQueryAsync();
                        
                        // Clean up tokens
                        SqliteCommand deleteCmd = connection.CreateCommand();
                        deleteCmd.CommandText = "DELETE FROM PasswordResetTokens WHERE UserId = $userId";
                        deleteCmd.Parameters.AddWithValue("$userId", userId);
                        await deleteCmd.ExecuteNonQueryAsync();
                        
                        return true;
                    }
                }
            }
        }
        return false;
    }
}
