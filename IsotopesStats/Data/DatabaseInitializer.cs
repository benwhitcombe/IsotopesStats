using Microsoft.Data.Sqlite;

namespace IsotopesStats.Data;

public static class DatabaseInitializer
{
    private const string ConnectionString = "Data Source=Data/IsotopesStats.db";

    public static async Task InitializeAsync()
    {
        using SqliteConnection connection = new SqliteConnection(ConnectionString);
        await connection.OpenAsync();

        using (SqliteCommand command = connection.CreateCommand())
        {
            command.CommandText = 
            @"
                CREATE TABLE IF NOT EXISTS Seasons (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    IsDeleted INTEGER NOT NULL DEFAULT 0
                );

                CREATE TABLE IF NOT EXISTS Players (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    IsDeleted INTEGER NOT NULL DEFAULT 0
                );

                CREATE TABLE IF NOT EXISTS SeasonPlayers (
                    SeasonId INTEGER NOT NULL,
                    PlayerId INTEGER NOT NULL,
                    PRIMARY KEY (SeasonId, PlayerId),
                    FOREIGN KEY (SeasonId) REFERENCES Seasons(Id),
                    FOREIGN KEY (PlayerId) REFERENCES Players(Id)
                );

                CREATE TABLE IF NOT EXISTS Games (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    SeasonId INTEGER NOT NULL DEFAULT 0,
                    GameNumber INTEGER NOT NULL DEFAULT 0,
                    Date TEXT NOT NULL,
                    Diamond TEXT NOT NULL DEFAULT '',
                    Opponent TEXT NOT NULL,
                    Type INTEGER NOT NULL DEFAULT 0,
                    IsDeleted INTEGER NOT NULL DEFAULT 0,
                    FOREIGN KEY (SeasonId) REFERENCES Seasons(Id)
                );

                CREATE TABLE IF NOT EXISTS Stats (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    PlayerId INTEGER NOT NULL,
                    GameId INTEGER NOT NULL,
                    BO INTEGER NOT NULL DEFAULT 0,
                    H1B INTEGER NOT NULL DEFAULT 0,
                    H2B INTEGER NOT NULL DEFAULT 0,
                    H3B INTEGER NOT NULL DEFAULT 0,
                    H4B INTEGER NOT NULL DEFAULT 0,
                    HR INTEGER NOT NULL DEFAULT 0,
                    FC INTEGER NOT NULL DEFAULT 0,
                    BB INTEGER NOT NULL DEFAULT 0,
                    SF INTEGER NOT NULL DEFAULT 0,
                    K INTEGER NOT NULL DEFAULT 0,
                    KF INTEGER NOT NULL DEFAULT 0,
                    GO INTEGER NOT NULL DEFAULT 0,
                    FO INTEGER NOT NULL DEFAULT 0,
                    R INTEGER NOT NULL DEFAULT 0,
                    RBI INTEGER NOT NULL DEFAULT 0,
                    FOREIGN KEY (PlayerId) REFERENCES Players(Id),
                    FOREIGN KEY (GameId) REFERENCES Games(Id)
                );

                CREATE TABLE IF NOT EXISTS Permissions (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL UNIQUE
                );

                CREATE TABLE IF NOT EXISTS UserRoles (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL UNIQUE,
                    IsDeleted INTEGER NOT NULL DEFAULT 0
                );

                CREATE TABLE IF NOT EXISTS RolePermissions (
                    RoleId INTEGER NOT NULL,
                    PermissionId INTEGER NOT NULL,
                    PRIMARY KEY (RoleId, PermissionId),
                    FOREIGN KEY (RoleId) REFERENCES UserRoles(Id),
                    FOREIGN KEY (PermissionId) REFERENCES Permissions(Id)
                );

                CREATE TABLE IF NOT EXISTS Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Email TEXT NOT NULL UNIQUE,
                    PasswordHash TEXT NOT NULL,
                    RoleId INTEGER NOT NULL DEFAULT 0,
                    CreatedAt TEXT NOT NULL,
                    IsDeleted INTEGER NOT NULL DEFAULT 0,
                    FOREIGN KEY (RoleId) REFERENCES UserRoles(Id)
                );

                CREATE TABLE IF NOT EXISTS PasswordResetTokens (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    UserId INTEGER NOT NULL,
                    Token TEXT NOT NULL,
                    ExpiryDate TEXT NOT NULL,
                    FOREIGN KEY (UserId) REFERENCES Users(Id)
                );
            ";
            await command.ExecuteNonQueryAsync();
        }
    }
}
