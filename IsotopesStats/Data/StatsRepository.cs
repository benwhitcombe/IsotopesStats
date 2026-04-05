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
            command.CommandText = "SELECT Id, Name, IsDeleted FROM Seasons WHERE IsDeleted = 0 ORDER BY Name DESC";

            using (SqliteDataReader reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    seasons.Add(new Season
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        IsDeleted = reader.GetInt32(2) == 1
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
                WHERE g.SeasonId = $seasonId AND p.Name NOT LIKE 'spare' AND g.IsDeleted = 0
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
                WHERE g.SeasonId = $seasonId AND g.IsDeleted = 0
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
            command.CommandText = 
            @"
                SELECT p.Id, p.Name, p.IsDeleted
                FROM Players p 
                JOIN SeasonPlayers sp ON p.Id = sp.PlayerId 
                WHERE sp.SeasonId = $seasonId AND p.IsDeleted = 0
                ORDER BY p.Name
            ";
            command.Parameters.AddWithValue("$seasonId", seasonId);

            using (SqliteDataReader reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    players.Add(new Player
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        IsDeleted = reader.GetInt32(2) == 1
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
                    p.Name, g.GameNumber, g.Date, g.Diamond, g.OpponentId, g.Type, g.IsDeleted,
                    o.Name as OpponentName
                FROM Stats s
                JOIN Players p ON s.PlayerId = p.Id
                JOIN Games g ON s.GameId = g.Id
                LEFT JOIN Opponents o ON g.OpponentId = o.Id
                WHERE g.SeasonId = $seasonId AND g.IsDeleted = 0
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
                            OpponentId = reader.GetInt32(22),
                            Type = (GameType)reader.GetInt32(23),
                            IsDeleted = reader.GetInt32(24) == 1,
                            Opponent = new Opponent { Id = reader.GetInt32(22), Name = reader.IsDBNull(25) ? "Unknown" : reader.GetString(25) }
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
                    g.GameNumber, g.Date, g.Diamond, g.OpponentId, g.Type, g.IsDeleted,
                    o.Name as OpponentName
                FROM Stats s
                JOIN Games g ON s.GameId = g.Id
                JOIN Players p ON s.PlayerId = p.Id
                LEFT JOIN Opponents o ON g.OpponentId = o.Id
                WHERE p.Name = $playerName AND g.SeasonId = $seasonId AND g.IsDeleted = 0
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
                            OpponentId = reader.GetInt32(21),
                            Type = (GameType)reader.GetInt32(22),
                            IsDeleted = reader.GetInt32(23) == 1,
                            Opponent = new Opponent { Id = reader.GetInt32(21), Name = reader.IsDBNull(24) ? "Unknown" : reader.GetString(24) }
                        }
                    });
                }
            }
        }
        return stats;
    }

    public async Task<Game?> GetGameAsync(int gameId)
    {
        using (SqliteConnection connection = new SqliteConnection(ConnectionString))
        {
            await connection.OpenAsync();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = 
            @"
                SELECT g.Id, g.SeasonId, g.GameNumber, g.Date, g.Diamond, g.OpponentId, g.Type, g.IsDeleted,
                       o.Name as OpponentName
                FROM Games g 
                LEFT JOIN Opponents o ON g.OpponentId = o.Id
                WHERE g.Id = $id AND g.IsDeleted = 0
            ";
            command.Parameters.AddWithValue("$id", gameId);

            using (SqliteDataReader reader = await command.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    return new Game
                    {
                        Id = reader.GetInt32(0),
                        SeasonId = reader.GetInt32(1),
                        GameNumber = reader.GetInt32(2),
                        Date = DateTime.Parse(reader.GetString(3)),
                        Diamond = reader.GetString(4),
                        OpponentId = reader.GetInt32(5),
                        Type = (GameType)reader.GetInt32(6),
                        IsDeleted = reader.GetInt32(7) == 1,
                        Opponent = new Opponent { Id = reader.GetInt32(5), Name = reader.IsDBNull(8) ? "Unknown" : reader.GetString(8) }
                    };
                }
            }
        }
        return null;
    }

    public async Task<List<StatEntry>> GetGameStatsAsync(int gameId)
    {
        List<StatEntry> stats = new List<StatEntry>();
        using (SqliteConnection connection = new SqliteConnection(ConnectionString))
        {
            await connection.OpenAsync();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = 
            @"
                SELECT s.Id, s.PlayerId, s.GameId, s.BO, s.H1B, s.H2B, s.H3B, s.H4B, s.HR, s.FC, s.BB, s.SF, s.K, s.KF, s.GO, s.FO, s.R, s.RBI,
                       p.Name
                FROM Stats s
                JOIN Players p ON s.PlayerId = p.Id
                WHERE s.GameId = $gameId
            ";
            command.Parameters.AddWithValue("$gameId", gameId);

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
                        Player = new Player
                        {
                            Id = reader.GetInt32(1),
                            Name = reader.GetString(18)
                        }
                    });
                }
            }
        }
        return stats;
    }

    public async Task UpdateGameWithStatsAsync(Game game, List<StatEntry> stats)
    {
        using (SqliteConnection connection = new SqliteConnection(ConnectionString))
        {
            await connection.OpenAsync();
            using SqliteTransaction transaction = connection.BeginTransaction();

            try
            {
                // Update Game
                SqliteCommand gameCommand = connection.CreateCommand();
                gameCommand.Transaction = transaction;
                gameCommand.CommandText = 
                @"
                    UPDATE Games SET 
                        SeasonId = $seasonId, 
                        GameNumber = $gameNumber, 
                        Date = $date, 
                        Diamond = $diamond, 
                        OpponentId = $opponentId, 
                        Type = $type
                    WHERE Id = $id
                ";
                gameCommand.Parameters.AddWithValue("$seasonId", game.SeasonId);
                gameCommand.Parameters.AddWithValue("$gameNumber", game.GameNumber);
                gameCommand.Parameters.AddWithValue("$date", game.Date.ToString("yyyy-MM-dd HH:mm:ss"));
                gameCommand.Parameters.AddWithValue("$diamond", game.Diamond);
                gameCommand.Parameters.AddWithValue("$opponentId", game.OpponentId);
                gameCommand.Parameters.AddWithValue("$type", (int)game.Type);
                gameCommand.Parameters.AddWithValue("$id", game.Id);
                await gameCommand.ExecuteNonQueryAsync();

                // Update Stats (Delete and re-insert is simplest for this scale)
                SqliteCommand deleteStats = connection.CreateCommand();
                deleteStats.Transaction = transaction;
                deleteStats.CommandText = "DELETE FROM Stats WHERE GameId = $gameId";
                deleteStats.Parameters.AddWithValue("$gameId", game.Id);
                await deleteStats.ExecuteNonQueryAsync();

                foreach (StatEntry stat in stats)
                {
                    SqliteCommand statCommand = connection.CreateCommand();
                    statCommand.Transaction = transaction;
                    statCommand.CommandText = 
                    @"
                        INSERT INTO Stats (PlayerId, GameId, BO, H1B, H2B, H3B, H4B, HR, FC, BB, SF, K, KF, GO, FO, R, RBI)
                        VALUES ($playerId, $gameId, $bo, $h1b, $h2b, $h3b, $h4b, $hr, $fc, $bb, $sf, $k, $kf, $go, $fo, $r, $rbi)
                    ";
                    statCommand.Parameters.AddWithValue("$playerId", stat.PlayerId);
                    statCommand.Parameters.AddWithValue("$gameId", game.Id);
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

                    // Ensure link in SeasonPlayers
                    SqliteCommand rosterCmd = connection.CreateCommand();
                    rosterCmd.Transaction = transaction;
                    rosterCmd.CommandText = "INSERT OR IGNORE INTO SeasonPlayers (SeasonId, PlayerId) VALUES ($seasonId, $playerId)";
                    rosterCmd.Parameters.AddWithValue("$seasonId", game.SeasonId);
                    rosterCmd.Parameters.AddWithValue("$playerId", stat.PlayerId);
                    await rosterCmd.ExecuteNonQueryAsync();
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
                    INSERT INTO Games (SeasonId, GameNumber, Date, Diamond, OpponentId, Type, IsDeleted)
                    VALUES ($seasonId, $gameNumber, $date, $diamond, $opponentId, $type, 0);
                    SELECT last_insert_rowid();
                ";
                gameCommand.Parameters.AddWithValue("$seasonId", game.SeasonId);
                gameCommand.Parameters.AddWithValue("$gameNumber", game.GameNumber);
                gameCommand.Parameters.AddWithValue("$date", game.Date.ToString("yyyy-MM-dd HH:mm:ss"));
                gameCommand.Parameters.AddWithValue("$diamond", game.Diamond);
                gameCommand.Parameters.AddWithValue("$opponentId", game.OpponentId);
                gameCommand.Parameters.AddWithValue("$type", (int)game.Type);

                int gameId = Convert.ToInt32(await gameCommand.ExecuteScalarAsync());

                foreach (StatEntry stat in stats)
                {
                    // Get or Create Player (Global)
                    SqliteCommand playerCommand = connection.CreateCommand();
                    playerCommand.Transaction = transaction;
                    playerCommand.CommandText = "SELECT Id FROM Players WHERE Name = $name AND IsDeleted = 0";
                    playerCommand.Parameters.AddWithValue("$name", stat.Player?.Name ?? "Unknown");
                    
                    object? playerIdObj = await playerCommand.ExecuteScalarAsync();
                    int playerId;

                    if (playerIdObj == null)
                    {
                        SqliteCommand insertPlayer = connection.CreateCommand();
                        insertPlayer.Transaction = transaction;
                        insertPlayer.CommandText = "INSERT INTO Players (Name, IsDeleted) VALUES ($name, 0); SELECT last_insert_rowid();";
                        insertPlayer.Parameters.AddWithValue("$name", stat.Player?.Name ?? "Unknown");
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

                    // Ensure link in SeasonPlayers
                    SqliteCommand rosterCmd = connection.CreateCommand();
                    rosterCmd.Transaction = transaction;
                    rosterCmd.CommandText = "INSERT OR IGNORE INTO SeasonPlayers (SeasonId, PlayerId) VALUES ($seasonId, $playerId)";
                    rosterCmd.Parameters.AddWithValue("$seasonId", game.SeasonId);
                    rosterCmd.Parameters.AddWithValue("$playerId", playerId);
                    await rosterCmd.ExecuteNonQueryAsync();
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

    public async Task UpdateGameAsync(Game game)
    {
        using (SqliteConnection connection = new SqliteConnection(ConnectionString))
        {
            await connection.OpenAsync();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = 
            @"
                UPDATE Games SET 
                    SeasonId = $seasonId, 
                    GameNumber = $gameNumber, 
                    Date = $date, 
                    Diamond = $diamond, 
                    OpponentId = $opponentId, 
                    Type = $type
                WHERE Id = $id
            ";
            command.Parameters.AddWithValue("$seasonId", game.SeasonId);
            command.Parameters.AddWithValue("$gameNumber", game.GameNumber);
            command.Parameters.AddWithValue("$date", game.Date.ToString("yyyy-MM-dd HH:mm:ss"));
            command.Parameters.AddWithValue("$diamond", game.Diamond);
            command.Parameters.AddWithValue("$opponentId", game.OpponentId);
            command.Parameters.AddWithValue("$type", (int)game.Type);
            command.Parameters.AddWithValue("$id", game.Id);
            await command.ExecuteNonQueryAsync();
        }
    }

    public async Task DeleteGameAsync(int gameId)
    {
        using (SqliteConnection connection = new SqliteConnection(ConnectionString))
        {
            await connection.OpenAsync();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = "UPDATE Games SET IsDeleted = 1 WHERE Id = $id";
            command.Parameters.AddWithValue("$id", gameId);
            await command.ExecuteNonQueryAsync();
        }
    }

    public async Task AddSeasonAsync(Season season)
    {
        using (SqliteConnection connection = new SqliteConnection(ConnectionString))
        {
            await connection.OpenAsync();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Seasons (Name, IsDeleted) VALUES ($name, 0)";
            command.Parameters.AddWithValue("$name", season.Name);
            await command.ExecuteNonQueryAsync();
        }
    }

    public async Task UpdateSeasonAsync(Season season)
    {
        using (SqliteConnection connection = new SqliteConnection(ConnectionString))
        {
            await connection.OpenAsync();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = "UPDATE Seasons SET Name = $name WHERE Id = $id";
            command.Parameters.AddWithValue("$name", season.Name);
            command.Parameters.AddWithValue("$id", season.Id);
            await command.ExecuteNonQueryAsync();
        }
    }

    public async Task DeleteSeasonAsync(int seasonId)
    {
        using (SqliteConnection connection = new SqliteConnection(ConnectionString))
        {
            await connection.OpenAsync();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = "UPDATE Seasons SET IsDeleted = 1 WHERE Id = $id";
            command.Parameters.AddWithValue("$id", seasonId);
            await command.ExecuteNonQueryAsync();
        }
    }

    public async Task AddPlayerToSeasonAsync(int playerId, int seasonId)
    {
        using (SqliteConnection connection = new SqliteConnection(ConnectionString))
        {
            await connection.OpenAsync();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = "INSERT OR IGNORE INTO SeasonPlayers (SeasonId, PlayerId) VALUES ($seasonId, $playerId)";
            command.Parameters.AddWithValue("$seasonId", seasonId);
            command.Parameters.AddWithValue("$playerId", playerId);
            await command.ExecuteNonQueryAsync();
        }
    }

    public async Task<List<Season>> GetSeasonsForPlayerAsync(int playerId)
    {
        List<Season> seasons = new List<Season>();
        using (SqliteConnection connection = new SqliteConnection(ConnectionString))
        {
            await connection.OpenAsync();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = 
            @"
                SELECT s.Id, s.Name, s.IsDeleted
                FROM Seasons s
                JOIN SeasonPlayers sp ON s.Id = sp.SeasonId
                WHERE sp.PlayerId = $playerId AND s.IsDeleted = 0
                ORDER BY s.Name DESC
            ";
            command.Parameters.AddWithValue("$playerId", playerId);

            using (SqliteDataReader reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    seasons.Add(new Season
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        IsDeleted = reader.GetInt32(2) == 1
                    });
                }
            }
        }
        return seasons;
    }

    public async Task AddPlayerAsync(Player player, int seasonId)
    {
        using (SqliteConnection connection = new SqliteConnection(ConnectionString))
        {
            await connection.OpenAsync();
            using SqliteTransaction transaction = connection.BeginTransaction();
            try
            {
                // 1. Get or Create Player
                SqliteCommand checkCmd = connection.CreateCommand();
                checkCmd.Transaction = transaction;
                checkCmd.CommandText = "SELECT Id FROM Players WHERE Name = $name AND IsDeleted = 0";
                checkCmd.Parameters.AddWithValue("$name", player.Name);
                object? id = await checkCmd.ExecuteScalarAsync();
                int playerId;

                if (id == null)
                {
                    SqliteCommand insertCmd = connection.CreateCommand();
                    insertCmd.Transaction = transaction;
                    insertCmd.CommandText = "INSERT INTO Players (Name, IsDeleted) VALUES ($name, 0); SELECT last_insert_rowid();";
                    insertCmd.Parameters.AddWithValue("$name", player.Name);
                    playerId = Convert.ToInt32(await insertCmd.ExecuteScalarAsync());
                }
                else
                {
                    playerId = Convert.ToInt32(id);
                }

                // 2. Add to Season roster
                SqliteCommand rosterCmd = connection.CreateCommand();
                rosterCmd.Transaction = transaction;
                rosterCmd.CommandText = "INSERT OR IGNORE INTO SeasonPlayers (SeasonId, PlayerId) VALUES ($seasonId, $playerId)";
                rosterCmd.Parameters.AddWithValue("$seasonId", seasonId);
                rosterCmd.Parameters.AddWithValue("$playerId", playerId);
                await rosterCmd.ExecuteNonQueryAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }

    public async Task UpdatePlayerAsync(Player player)
    {
        using (SqliteConnection connection = new SqliteConnection(ConnectionString))
        {
            await connection.OpenAsync();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = "UPDATE Players SET Name = $name WHERE Id = $id";
            command.Parameters.AddWithValue("$name", player.Name);
            command.Parameters.AddWithValue("$id", player.Id);
            await command.ExecuteNonQueryAsync();
        }
    }

    public async Task DeletePlayerAsync(int playerId)
    {
        using (SqliteConnection connection = new SqliteConnection(ConnectionString))
        {
            await connection.OpenAsync();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = "UPDATE Players SET IsDeleted = 1 WHERE Id = $id";
            command.Parameters.AddWithValue("$id", playerId);
            await command.ExecuteNonQueryAsync();
        }
    }

    public async Task DeletePlayerFromSeasonAsync(int playerId, int seasonId)
    {
        using (SqliteConnection connection = new SqliteConnection(ConnectionString))
        {
            await connection.OpenAsync();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = "DELETE FROM SeasonPlayers WHERE PlayerId = $playerId AND SeasonId = $seasonId";
            command.Parameters.AddWithValue("$playerId", playerId);
            command.Parameters.AddWithValue("$seasonId", seasonId);
            await command.ExecuteNonQueryAsync();
        }
    }

    public async Task<List<Player>> GetAllPlayersAsync()
    {
        List<Player> players = new List<Player>();
        using (SqliteConnection connection = new SqliteConnection(ConnectionString))
        {
            await connection.OpenAsync();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Name, IsDeleted FROM Players WHERE IsDeleted = 0 ORDER BY Name";

            using (SqliteDataReader reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    players.Add(new Player
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        IsDeleted = reader.GetInt32(2) == 1
                    });
                }
            }
        }
        return players;
    }

    public async Task<List<Opponent>> GetOpponentsAsync(int seasonId)
    {
        List<Opponent> opponents = new List<Opponent>();
        using (SqliteConnection connection = new SqliteConnection(ConnectionString))
        {
            await connection.OpenAsync();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = 
            @"
                SELECT o.Id, o.Name, o.IsDeleted
                FROM Opponents o 
                JOIN SeasonOpponents so ON o.Id = so.OpponentId 
                WHERE so.SeasonId = $seasonId AND o.IsDeleted = 0
                ORDER BY o.Name
            ";
            command.Parameters.AddWithValue("$seasonId", seasonId);

            using (SqliteDataReader reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    opponents.Add(new Opponent
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        IsDeleted = reader.GetInt32(2) == 1
                    });
                }
            }
        }
        return opponents;
    }

    public async Task<List<Opponent>> GetAllOpponentsAsync()
    {
        List<Opponent> opponents = new List<Opponent>();
        using (SqliteConnection connection = new SqliteConnection(ConnectionString))
        {
            await connection.OpenAsync();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Name, IsDeleted FROM Opponents WHERE IsDeleted = 0 ORDER BY Name";

            using (SqliteDataReader reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    opponents.Add(new Opponent
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        IsDeleted = reader.GetInt32(2) == 1
                    });
                }
            }
        }
        return opponents;
    }

    public async Task AddOpponentAsync(Opponent opponent, int seasonId)
    {
        using (SqliteConnection connection = new SqliteConnection(ConnectionString))
        {
            await connection.OpenAsync();
            using SqliteTransaction transaction = connection.BeginTransaction();
            try
            {
                SqliteCommand checkCmd = connection.CreateCommand();
                checkCmd.Transaction = transaction;
                checkCmd.CommandText = "SELECT Id FROM Opponents WHERE Name = $name AND IsDeleted = 0";
                checkCmd.Parameters.AddWithValue("$name", opponent.Name);
                object? id = await checkCmd.ExecuteScalarAsync();
                int opponentId;

                if (id == null)
                {
                    SqliteCommand insertCmd = connection.CreateCommand();
                    insertCmd.Transaction = transaction;
                    insertCmd.CommandText = "INSERT INTO Opponents (Name, IsDeleted) VALUES ($name, 0); SELECT last_insert_rowid();";
                    insertCmd.Parameters.AddWithValue("$name", opponent.Name);
                    opponentId = Convert.ToInt32(await insertCmd.ExecuteScalarAsync());
                }
                else
                {
                    opponentId = Convert.ToInt32(id);
                }

                if (seasonId != 0)
                {
                    SqliteCommand rosterCmd = connection.CreateCommand();
                    rosterCmd.Transaction = transaction;
                    rosterCmd.CommandText = "INSERT OR IGNORE INTO SeasonOpponents (SeasonId, OpponentId) VALUES ($seasonId, $opponentId)";
                    rosterCmd.Parameters.AddWithValue("$seasonId", seasonId);
                    rosterCmd.Parameters.AddWithValue("$opponentId", opponentId);
                    await rosterCmd.ExecuteNonQueryAsync();
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

    public async Task UpdateOpponentAsync(Opponent opponent)
    {
        using (SqliteConnection connection = new SqliteConnection(ConnectionString))
        {
            await connection.OpenAsync();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = "UPDATE Opponents SET Name = $name WHERE Id = $id";
            command.Parameters.AddWithValue("$name", opponent.Name);
            command.Parameters.AddWithValue("$id", opponent.Id);
            await command.ExecuteNonQueryAsync();
        }
    }

    public async Task DeleteOpponentAsync(int opponentId)
    {
        using (SqliteConnection connection = new SqliteConnection(ConnectionString))
        {
            await connection.OpenAsync();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = "UPDATE Opponents SET IsDeleted = 1 WHERE Id = $id";
            command.Parameters.AddWithValue("$id", opponentId);
            await command.ExecuteNonQueryAsync();
        }
    }

    public async Task AddOpponentToSeasonAsync(int opponentId, int seasonId)
    {
        using (SqliteConnection connection = new SqliteConnection(ConnectionString))
        {
            await connection.OpenAsync();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = "INSERT OR IGNORE INTO SeasonOpponents (SeasonId, OpponentId) VALUES ($seasonId, $opponentId)";
            command.Parameters.AddWithValue("$seasonId", seasonId);
            command.Parameters.AddWithValue("$opponentId", opponentId);
            await command.ExecuteNonQueryAsync();
        }
    }

    public async Task DeleteOpponentFromSeasonAsync(int opponentId, int seasonId)
    {
        using (SqliteConnection connection = new SqliteConnection(ConnectionString))
        {
            await connection.OpenAsync();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = "DELETE FROM SeasonOpponents WHERE OpponentId = $opponentId AND SeasonId = $seasonId";
            command.Parameters.AddWithValue("$opponentId", opponentId);
            command.Parameters.AddWithValue("$seasonId", seasonId);
            await command.ExecuteNonQueryAsync();
        }
    }

    public async Task<List<Season>> GetSeasonsForOpponentAsync(int opponentId)
    {
        List<Season> seasons = new List<Season>();
        using (SqliteConnection connection = new SqliteConnection(ConnectionString))
        {
            await connection.OpenAsync();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = 
            @"
                SELECT s.Id, s.Name, s.IsDeleted
                FROM Seasons s
                JOIN SeasonOpponents so ON s.Id = so.SeasonId
                WHERE so.OpponentId = $opponentId AND s.IsDeleted = 0
                ORDER BY s.Name DESC
            ";
            command.Parameters.AddWithValue("$opponentId", opponentId);

            using (SqliteDataReader reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    seasons.Add(new Season
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        IsDeleted = reader.GetInt32(2) == 1
                    });
                }
            }
        }
        return seasons;
    }
}
