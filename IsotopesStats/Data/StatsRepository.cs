using Microsoft.Data.Sqlite;
using IsotopesStats.Models;

namespace IsotopesStats.Data;

public class StatsRepository
{
    private const string ConnectionString = "Data Source=Data/IsotopesStats.db";

    public async Task<List<Season>> GetSeasonsAsync()
    {
        List<Season> seasons = new List<Season>();
        using (SqliteConnection connection = new SqliteConnection(ConnectionString))
        {
            await connection.OpenAsync();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Name FROM Seasons ORDER BY Name DESC";

            using (SqliteDataReader reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    seasons.Add(new Season
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1)
                    });
                }
            }
        }
        return seasons;
    }

    public async Task<List<PlayerStatsSummary>> GetStatsSummaryAsync(int seasonId)
    {
        List<PlayerStatsSummary> stats = new List<PlayerStatsSummary>();
        using (SqliteConnection connection = new SqliteConnection(ConnectionString))
        {
            await connection.OpenAsync();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = 
            @"
                SELECT 
                    p.Name as PlayerName,
                    COUNT(DISTINCT s.GameId) as GamesPlayed,
                    SUM(s.H1B) as H1B,
                    SUM(s.H2B) as H2B,
                    SUM(s.H3B) as H3B,
                    SUM(s.H4B) as H4B,
                    SUM(s.HR) as HR,
                    SUM(s.RBI) as RBI,
                    SUM(s.R) as R,
                    SUM(s.BB) as BB,
                    SUM(s.K) as K,
                    SUM(s.KF) as KF,
                    SUM(s.SF) as SF,
                    SUM(s.FC) as FC,
                    SUM(s.GO) as GO,
                    SUM(s.FO) as FO
                FROM Stats s
                JOIN Players p ON s.PlayerId = p.Id
                JOIN Games g ON s.GameId = g.Id
                WHERE g.SeasonId = $seasonId
                GROUP BY p.Id, p.Name
            ";
            command.Parameters.AddWithValue("$seasonId", seasonId);

            using (SqliteDataReader reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    stats.Add(new PlayerStatsSummary
                    {
                        PlayerName = reader.GetString(0),
                        GamesPlayed = reader.GetInt32(1),
                        H1B = reader.GetInt32(2),
                        H2B = reader.GetInt32(3),
                        H3B = reader.GetInt32(4),
                        H4B = reader.GetInt32(5),
                        HR = reader.GetInt32(6),
                        RBI = reader.GetInt32(7),
                        R = reader.GetInt32(8),
                        BB = reader.GetInt32(9),
                        K = reader.GetInt32(10),
                        KF = reader.GetInt32(11),
                        SF = reader.GetInt32(12),
                        FC = reader.GetInt32(13),
                        GO = reader.GetInt32(14),
                        FO = reader.GetInt32(15)
                    });
                }
            }
        }
        return stats;
    }

    public async Task<PlayerStatsSummary> GetTeamTotalsAsync(int seasonId)
    {
        using (SqliteConnection connection = new SqliteConnection(ConnectionString))
        {
            await connection.OpenAsync();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = 
            @"
                SELECT 
                    SUM(s.H1B) as H1B,
                    SUM(s.H2B) as H2B,
                    SUM(s.H3B) as H3B,
                    SUM(s.H4B) as H4B,
                    SUM(s.HR) as HR,
                    SUM(s.RBI) as RBI,
                    SUM(s.R) as R,
                    SUM(s.BB) as BB,
                    SUM(s.K) as K,
                    SUM(s.KF) as KF,
                    SUM(s.SF) as SF,
                    SUM(s.FC) as FC,
                    SUM(s.GO) as GO,
                    SUM(s.FO) as FO
                FROM Stats s
                JOIN Games g ON s.GameId = g.Id
                WHERE g.SeasonId = $seasonId
            ";
            command.Parameters.AddWithValue("$seasonId", seasonId);

            using (SqliteDataReader reader = await command.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    return new PlayerStatsSummary
                    {
                        PlayerName = "TEAM TOTALS",
                        H1B = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                        H2B = reader.IsDBNull(1) ? 0 : reader.GetInt32(1),
                        H3B = reader.IsDBNull(2) ? 0 : reader.GetInt32(2),
                        H4B = reader.IsDBNull(3) ? 0 : reader.GetInt32(3),
                        HR = reader.IsDBNull(4) ? 0 : reader.GetInt32(4),
                        RBI = reader.IsDBNull(5) ? 0 : reader.GetInt32(5),
                        R = reader.IsDBNull(6) ? 0 : reader.GetInt32(6),
                        BB = reader.IsDBNull(7) ? 0 : reader.GetInt32(7),
                        K = reader.IsDBNull(8) ? 0 : reader.GetInt32(8),
                        KF = reader.IsDBNull(9) ? 0 : reader.GetInt32(9),
                        SF = reader.IsDBNull(10) ? 0 : reader.GetInt32(10),
                        FC = reader.IsDBNull(11) ? 0 : reader.GetInt32(11),
                        GO = reader.IsDBNull(12) ? 0 : reader.GetInt32(12),
                        FO = reader.IsDBNull(13) ? 0 : reader.GetInt32(13)
                    };
                }
            }
        }
        return new PlayerStatsSummary { PlayerName = "TEAM TOTALS" };
    }

    public async Task<List<Player>> GetPlayersAsync(int seasonId)
    {
        List<Player> players = new List<Player>();
        using (SqliteConnection connection = new SqliteConnection(ConnectionString))
        {
            await connection.OpenAsync();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = "SELECT DISTINCT p.Id, p.Name FROM Players p JOIN Stats s ON p.Id = s.PlayerId JOIN Games g ON s.GameId = g.Id WHERE g.SeasonId = $seasonId ORDER BY p.Name";
            command.Parameters.AddWithValue("$seasonId", seasonId);

            using (SqliteDataReader reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    players.Add(new Player
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1)
                    });
                }
            }
        }
        return players;
    }

    public async Task<List<StatEntry>> GetAllGameStatsAsync(int seasonId)
    {
        List<StatEntry> stats = new List<StatEntry>();
        using (SqliteConnection connection = new SqliteConnection(ConnectionString))
        {
            await connection.OpenAsync();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = 
            @"
                SELECT 
                    s.Id, s.PlayerId, s.GameId, s.BO, s.H1B, s.H2B, s.H3B, s.H4B, s.HR, s.FC, s.BB, s.SF, s.K, s.KF, s.GO, s.FO, s.R, s.RBI,
                    p.Name, g.GameNumber, g.Date, g.Diamond, g.Opponent, g.Type
                FROM Stats s
                JOIN Players p ON s.PlayerId = p.Id
                JOIN Games g ON s.GameId = g.Id
                WHERE g.SeasonId = $seasonId
                ORDER BY g.Date DESC, s.BO ASC
            ";
            command.Parameters.AddWithValue("$seasonId", seasonId);

            using (SqliteDataReader reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    stats.Add(new StatEntry
                    {
                        Id = reader.GetInt32(0),
                        PlayerId = reader.GetInt32(1),
                        GameId = reader.GetInt32(2),
                        BO = reader.GetInt32(3),
                        H1B = reader.GetInt32(4),
                        H2B = reader.GetInt32(5),
                        H3B = reader.GetInt32(6),
                        H4B = reader.GetInt32(7),
                        HR = reader.GetInt32(8),
                        FC = reader.GetInt32(9),
                        BB = reader.GetInt32(10),
                        SF = reader.GetInt32(11),
                        K = reader.GetInt32(12),
                        KF = reader.GetInt32(13),
                        GO = reader.GetInt32(14),
                        FO = reader.GetInt32(15),
                        R = reader.GetInt32(16),
                        RBI = reader.GetInt32(17),
                        Player = new Player { Id = reader.GetInt32(1), Name = reader.GetString(18) },
                        Game = new Game
                        {
                            Id = reader.GetInt32(2),
                            SeasonId = seasonId,
                            GameNumber = reader.GetInt32(19),
                            Date = DateTime.Parse(reader.GetString(20)),
                            Diamond = reader.GetString(21),
                            Opponent = reader.GetString(22),
                            Type = (GameType)reader.GetInt32(23)
                        }
                    });
                }
            }
        }
        return stats;
    }

    public async Task<List<StatEntry>> GetPlayerGameLogAsync(string playerName, int seasonId)
    {
        List<StatEntry> stats = new List<StatEntry>();
        using (SqliteConnection connection = new SqliteConnection(ConnectionString))
        {
            await connection.OpenAsync();

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = 
            @"
                SELECT 
                    s.Id, s.PlayerId, s.GameId, s.BO, s.H1B, s.H2B, s.H3B, s.H4B, s.HR, s.FC, s.BB, s.SF, s.K, s.KF, s.GO, s.FO, s.R, s.RBI,
                    g.GameNumber, g.Date, g.Diamond, g.Opponent, g.Type
                FROM Stats s
                JOIN Games g ON s.GameId = g.Id
                JOIN Players p ON s.PlayerId = p.Id
                WHERE p.Name = $playerName AND g.SeasonId = $seasonId
                ORDER BY g.Date DESC
            ";
            command.Parameters.AddWithValue("$playerName", playerName);
            command.Parameters.AddWithValue("$seasonId", seasonId);

            using (SqliteDataReader reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    stats.Add(new StatEntry
                    {
                        Id = reader.GetInt32(0),
                        PlayerId = reader.GetInt32(1),
                        GameId = reader.GetInt32(2),
                        BO = reader.GetInt32(3),
                        H1B = reader.GetInt32(4),
                        H2B = reader.GetInt32(5),
                        H3B = reader.GetInt32(6),
                        H4B = reader.GetInt32(7),
                        HR = reader.GetInt32(8),
                        FC = reader.GetInt32(9),
                        BB = reader.GetInt32(10),
                        SF = reader.GetInt32(11),
                        K = reader.GetInt32(12),
                        KF = reader.GetInt32(13),
                        GO = reader.GetInt32(14),
                        FO = reader.GetInt32(15),
                        R = reader.GetInt32(16),
                        RBI = reader.GetInt32(17),
                        Game = new Game
                        {
                            Id = reader.GetInt32(2),
                            SeasonId = seasonId,
                            GameNumber = reader.GetInt32(18),
                            Date = DateTime.Parse(reader.GetString(19)),
                            Diamond = reader.GetString(20),
                            Opponent = reader.GetString(21),
                            Type = (GameType)reader.GetInt32(22)
                        }
                    });
                }
            }
        }
        return stats;
    }

    public async Task AddGameWithStatsAsync(Game game, List<StatEntry> stats)
    {
        using (SqliteConnection connection = new SqliteConnection(ConnectionString))
        {
            await connection.OpenAsync();
            using SqliteTransaction transaction = connection.BeginTransaction();

            try
            {
                SqliteCommand gameCommand = connection.CreateCommand();
                gameCommand.Transaction = transaction;
                gameCommand.CommandText = 
                @"
                    INSERT INTO Games (SeasonId, GameNumber, Date, Diamond, Opponent, Type)
                    VALUES ($seasonId, $gameNumber, $date, $diamond, $opponent, $type);
                    SELECT last_insert_rowid();
                ";
                gameCommand.Parameters.AddWithValue("$seasonId", game.SeasonId);
                gameCommand.Parameters.AddWithValue("$gameNumber", game.GameNumber);
                gameCommand.Parameters.AddWithValue("$date", game.Date.ToString("yyyy-MM-dd HH:mm:ss"));
                gameCommand.Parameters.AddWithValue("$diamond", game.Diamond);
                gameCommand.Parameters.AddWithValue("$opponent", game.Opponent);
                gameCommand.Parameters.AddWithValue("$type", (int)game.Type);

                int gameId = Convert.ToInt32(await gameCommand.ExecuteScalarAsync());

                foreach (StatEntry stat in stats)
                {
                    // Get or Create Player
                    SqliteCommand playerCommand = connection.CreateCommand();
                    playerCommand.Transaction = transaction;
                    playerCommand.CommandText = "SELECT Id FROM Players WHERE Name = $name";
                    playerCommand.Parameters.AddWithValue("$name", stat.Player?.Name ?? "Unknown");
                    
                    object? playerIdObj = await playerCommand.ExecuteScalarAsync();
                    int playerId;

                    if (playerIdObj == null)
                    {
                        SqliteCommand insertPlayer = connection.CreateCommand();
                        insertPlayer.Transaction = transaction;
                        insertPlayer.CommandText = "INSERT INTO Players (Name, SeasonId) VALUES ($name, $seasonId); SELECT last_insert_rowid();";
                        insertPlayer.Parameters.AddWithValue("$name", stat.Player?.Name ?? "Unknown");
                        insertPlayer.Parameters.AddWithValue("$seasonId", game.SeasonId);
                        playerId = Convert.ToInt32(await insertPlayer.ExecuteScalarAsync());
                    }
                    else
                    {
                        playerId = Convert.ToInt32(playerIdObj);
                    }

                    // Insert Stat
                    SqliteCommand statCommand = connection.CreateCommand();
                    statCommand.Transaction = transaction;
                    statCommand.CommandText = 
                    @"
                        INSERT INTO Stats (PlayerId, GameId, BO, H1B, H2B, H3B, H4B, HR, FC, BB, SF, K, KF, GO, FO, R, RBI)
                        VALUES ($playerId, $gameId, $bo, $h1b, $h2b, $h3b, $h4b, $hr, $fc, $bb, $sf, $k, $kf, $go, $fo, $r, $rbi)
                    ";
                    statCommand.Parameters.AddWithValue("$playerId", playerId);
                    statCommand.Parameters.AddWithValue("$gameId", gameId);
                    statCommand.Parameters.AddWithValue("$bo", stat.BO);
                    statCommand.Parameters.AddWithValue("$h1b", stat.H1B);
                    statCommand.Parameters.AddWithValue("$h2b", stat.H2B);
                    statCommand.Parameters.AddWithValue("$h3b", stat.H3B);
                    statCommand.Parameters.AddWithValue("$h4b", stat.H4B);
                    statCommand.Parameters.AddWithValue("$hr", stat.HR);
                    statCommand.Parameters.AddWithValue("$fc", stat.FC);
                    statCommand.Parameters.AddWithValue("$bb", stat.BB);
                    statCommand.Parameters.AddWithValue("$sf", stat.SF);
                    statCommand.Parameters.AddWithValue("$k", stat.K);
                    statCommand.Parameters.AddWithValue("$kf", stat.KF);
                    statCommand.Parameters.AddWithValue("$go", stat.GO);
                    statCommand.Parameters.AddWithValue("$fo", stat.FO);
                    statCommand.Parameters.AddWithValue("$r", stat.R);
                    statCommand.Parameters.AddWithValue("$rbi", stat.RBI);
                    await statCommand.ExecuteNonQueryAsync();
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
}
