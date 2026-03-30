using Microsoft.Data.Sqlite;

namespace IsotopesStats.Data;

public static class DatabaseInitializer
{
    private const string ConnectionString = "Data Source=Data/IsotopesStats.db";

    public static void Initialize()
    {
        using SqliteConnection connection = new SqliteConnection(ConnectionString);
        connection.Open();

        SqliteCommand command = connection.CreateCommand();
        command.CommandText = 
        @"
            CREATE TABLE IF NOT EXISTS Seasons (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS Players (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                SeasonId INTEGER NOT NULL DEFAULT 0,
                Name TEXT NOT NULL,
                IsActive INTEGER NOT NULL DEFAULT 1,
                FOREIGN KEY (SeasonId) REFERENCES Seasons(Id)
            );

            CREATE TABLE IF NOT EXISTS Games (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                SeasonId INTEGER NOT NULL DEFAULT 0,
                GameNumber INTEGER NOT NULL DEFAULT 0,
                Date TEXT NOT NULL,
                Diamond TEXT NOT NULL DEFAULT '',
                Opponent TEXT NOT NULL,
                Type INTEGER NOT NULL DEFAULT 0,
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
        ";
        command.ExecuteNonQuery();
    }
}
