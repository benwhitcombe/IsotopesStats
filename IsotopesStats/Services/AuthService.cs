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
            command.CommandText = "SELECT Id, Email, PasswordHash, IsAdmin, CreatedAt FROM Users WHERE Email = $email";
            command.Parameters.AddWithValue("$email", email);

            using (SqliteDataReader reader = await command.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    string storedHash = reader.GetString(2);
                    if (BCrypt.Net.BCrypt.Verify(password, storedHash))
                    {
                        return new User
                        {
                            Id = reader.GetInt32(0),
                            Email = reader.GetString(1),
                            PasswordHash = storedHash,
                            IsAdmin = reader.GetInt32(3) == 1,
                            CreatedAt = DateTime.Parse(reader.GetString(4))
                        };
                    }
                }
            }
        }
        return null;
    }

    public async Task<bool> RegisterAsync(string email, string password, bool isAdmin = false)
    {
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
        using (SqliteConnection connection = new SqliteConnection(ConnectionString))
        {
            await connection.OpenAsync();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = 
            @"
                INSERT INTO Users (Email, PasswordHash, IsAdmin, CreatedAt)
                VALUES ($email, $hash, $isAdmin, $createdAt)
            ";
            command.Parameters.AddWithValue("$email", email);
            command.Parameters.AddWithValue("$hash", passwordHash);
            command.Parameters.AddWithValue("$isAdmin", isAdmin ? 1 : 0);
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

    public async Task<string> GeneratePasswordResetTokenAsync(string email)
    {
        using (SqliteConnection connection = new SqliteConnection(ConnectionString))
        {
            await connection.OpenAsync();
            
            // Check if user exists
            SqliteCommand userCmd = connection.CreateCommand();
            userCmd.CommandText = "SELECT Id FROM Users WHERE Email = $email";
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

    public async Task<List<User>> GetUsersAsync()
    {
        List<User> users = new List<User>();
        using (SqliteConnection connection = new SqliteConnection(ConnectionString))
        {
            await connection.OpenAsync();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Email, IsAdmin, CreatedAt FROM Users ORDER BY Email";

            using (SqliteDataReader reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    users.Add(new User
                    {
                        Id = reader.GetInt32(0),
                        Email = reader.GetString(1),
                        IsAdmin = reader.GetInt32(2) == 1,
                        CreatedAt = DateTime.Parse(reader.GetString(3))
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
            command.CommandText = "DELETE FROM Users WHERE Id = $id";
            command.Parameters.AddWithValue("$id", userId);
            await command.ExecuteNonQueryAsync();
        }
    }
}
