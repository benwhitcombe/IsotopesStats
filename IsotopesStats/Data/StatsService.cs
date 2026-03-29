using Microsoft.Data.Sqlite;
using IsotopesStats.Models;

namespace IsotopesStats.Data;

public class StatsService
{
    private const string ConnectionString = "Data Source=IsotopesStats.db";

    public void Seed2025Data()
    {
        using SqliteConnection connection = new SqliteConnection(ConnectionString);
        connection.Open();

        // Check if data already exists
        SqliteCommand checkCommand = connection.CreateCommand();
        checkCommand.CommandText = "SELECT COUNT(*) FROM Players";
        if (Convert.ToInt32(checkCommand.ExecuteScalar()) > 0) return;

        // 1. Create 2025 Season Summary Game
        SqliteCommand gameCommand = connection.CreateCommand();
        gameCommand.CommandText = "INSERT INTO Games (Date, Opponent, Type) VALUES ('2025-12-31', '2025 Season Summary', 0); SELECT last_insert_rowid();";
        int gameId = Convert.ToInt32(gameCommand.ExecuteScalar());

        // 2. Player Data (Player, GP, PA, AB, H, 1B, 2B, 3B, 4B, HR, FC, BB, SF, K, KF, B, GO, FO, O, R, RBI)
        List<(string Name, int GP, int PA, int AB, int H, int H1B, int H2B, int H3B, int H4B, int HR, int FC, int BB, int SF, int K, int KF, int B, int GO, int FO, int O, int R, int RBI)> players = new List<(string Name, int GP, int PA, int AB, int H, int H1B, int H2B, int H3B, int H4B, int HR, int FC, int BB, int SF, int K, int KF, int B, int GO, int FO, int O, int R, int RBI)>
        {
            ("Mitch", 21, 86, 77, 59, 43, 8, 5, 0, 3, 2, 5, 4, 1, 0, 86, 1, 14, 15, 42, 43),
            ("Justin", 31, 134, 119, 88, 44, 28, 5, 8, 3, 8, 10, 5, 0, 1, 159, 6, 16, 22, 78, 62),
            ("Mike", 26, 105, 94, 59, 42, 13, 1, 0, 3, 1, 7, 4, 0, 0, 83, 4, 30, 34, 40, 39),
            ("Tim P", 30, 123, 115, 72, 51, 14, 6, 1, 0, 4, 7, 1, 1, 0, 101, 5, 33, 38, 59, 36),
            ("Luke", 33, 126, 114, 70, 57, 10, 3, 0, 0, 5, 9, 3, 1, 1, 86, 4, 33, 37, 33, 28),
            ("Henry", 31, 131, 116, 71, 40, 12, 1, 0, 18, 2, 9, 6, 1, 4, 139, 1, 37, 38, 43, 75),
            ("Tim G", 8, 28, 28, 17, 11, 4, 2, 0, 0, 0, 0, 0, 0, 0, 25, 0, 11, 11, 9, 8),
            ("Jay", 23, 92, 85, 51, 32, 12, 4, 0, 3, 3, 4, 3, 0, 0, 80, 2, 29, 31, 38, 44),
            ("Dan", 31, 128, 117, 70, 45, 19, 3, 0, 3, 4, 3, 8, 0, 0, 104, 1, 42, 43, 51, 51),
            ("Hiren", 26, 95, 94, 56, 47, 6, 3, 0, 0, 7, 0, 1, 0, 0, 68, 9, 22, 31, 26, 33),
            ("Bill", 22, 82, 79, 43, 31, 9, 2, 0, 1, 3, 0, 3, 0, 1, 59, 2, 30, 32, 28, 34),
            ("Ben", 37, 137, 131, 63, 50, 11, 2, 0, 0, 16, 3, 3, 0, 0, 78, 5, 47, 52, 38, 43),
            ("Kevin", 23, 86, 80, 37, 32, 5, 0, 0, 0, 8, 4, 2, 1, 2, 42, 6, 26, 32, 16, 24),
            ("Doug", 37, 129, 123, 55, 49, 5, 1, 0, 0, 7, 6, 0, 0, 1, 62, 28, 32, 60, 39, 28),
            ("Suthan", 18, 61, 55, 22, 18, 3, 0, 0, 1, 4, 5, 1, 0, 1, 28, 4, 24, 28, 13, 13),
            ("Craig", 20, 72, 67, 21, 20, 1, 0, 0, 0, 4, 5, 0, 0, 0, 22, 8, 34, 42, 16, 12)
        };

        foreach ((string Name, int GP, int PA, int AB, int H, int H1B, int H2B, int H3B, int H4B, int HR, int FC, int BB, int SF, int K, int KF, int B, int GO, int FO, int O, int R, int RBI) player in players)
        {
            SqliteCommand playerCommand = connection.CreateCommand();
            playerCommand.CommandText = "INSERT INTO Players (Name) VALUES ($name); SELECT last_insert_rowid();";
            playerCommand.Parameters.AddWithValue("$name", player.Name);
            int playerId = Convert.ToInt32(playerCommand.ExecuteScalar());

            SqliteCommand statCommand = connection.CreateCommand();
            statCommand.CommandText = 
            @"
                INSERT INTO Stats (PlayerId, GameId, PA, AB, H, H1B, H2B, H3B, H4B, HR, FC, BB, SF, K, KF, B, GO, FO, O, R, RBI) 
                VALUES ($playerId, $gameId, $pa, $ab, $h, $h1b, $h2b, $h3b, $h4b, $hr, $fc, $bb, $sf, $k, $kf, $b, $go, $fo, $o, $r, $rbi)
            ";
            statCommand.Parameters.AddWithValue("$playerId", playerId);
            statCommand.Parameters.AddWithValue("$gameId", gameId);
            statCommand.Parameters.AddWithValue("$pa", player.PA);
            statCommand.Parameters.AddWithValue("$ab", player.AB);
            statCommand.Parameters.AddWithValue("$h", player.H);
            statCommand.Parameters.AddWithValue("$h1b", player.H1B);
            statCommand.Parameters.AddWithValue("$h2b", player.H2B);
            statCommand.Parameters.AddWithValue("$h3b", player.H3B);
            statCommand.Parameters.AddWithValue("$h4b", player.H4B);
            statCommand.Parameters.AddWithValue("$hr", player.HR);
            statCommand.Parameters.AddWithValue("$fc", player.FC);
            statCommand.Parameters.AddWithValue("$bb", player.BB);
            statCommand.Parameters.AddWithValue("$sf", player.SF);
            statCommand.Parameters.AddWithValue("$k", player.K);
            statCommand.Parameters.AddWithValue("$kf", player.KF);
            statCommand.Parameters.AddWithValue("$b", player.B);
            statCommand.Parameters.AddWithValue("$go", player.GO);
            statCommand.Parameters.AddWithValue("$fo", player.FO);
            statCommand.Parameters.AddWithValue("$o", player.O);
            statCommand.Parameters.AddWithValue("$r", player.R);
            statCommand.Parameters.AddWithValue("$rbi", player.RBI);
            statCommand.ExecuteNonQuery();
        }
    }

    public List<PlayerStatsSummary> GetStatsSummary()
    {
        List<PlayerStatsSummary> summaries = new List<PlayerStatsSummary>();
        using SqliteConnection connection = new SqliteConnection(ConnectionString);
        connection.Open();

        SqliteCommand command = connection.CreateCommand();
        command.CommandText = 
        @"
            SELECT 
                p.Name,
                COUNT(DISTINCT s.GameId) as GamesPlayed,
                SUM(s.PA) as PA,
                SUM(s.AB) as AB,
                SUM(s.H) as H,
                SUM(s.H1B) as H1B,
                SUM(s.H2B) as H2B,
                SUM(s.H3B) as H3B,
                SUM(s.H4B) as H4B,
                SUM(s.HR) as HR,
                SUM(s.FC) as FC,
                SUM(s.BB) as BB,
                SUM(s.SF) as SF,
                SUM(s.K) as K,
                SUM(s.KF) as KF,
                SUM(s.B) as B,
                SUM(s.GO) as GO,
                SUM(s.FO) as FO,
                SUM(s.O) as O,
                SUM(s.R) as R,
                SUM(s.RBI) as RBI
            FROM Players p
            LEFT JOIN Stats s ON p.Id = s.PlayerId
            GROUP BY p.Id, p.Name
            ORDER BY (CAST(SUM(s.H) AS FLOAT) / SUM(s.AB)) DESC
        ";

        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            summaries.Add(new PlayerStatsSummary
            {
                PlayerName = reader.GetString(0),
                GamesPlayed = Convert.ToInt32(reader.GetValue(1)),
                PA = reader.IsDBNull(2) ? 0 : Convert.ToInt32(reader.GetValue(2)),
                AB = reader.IsDBNull(3) ? 0 : Convert.ToInt32(reader.GetValue(3)),
                H = reader.IsDBNull(4) ? 0 : Convert.ToInt32(reader.GetValue(4)),
                H1B = reader.IsDBNull(5) ? 0 : Convert.ToInt32(reader.GetValue(5)),
                H2B = reader.IsDBNull(6) ? 0 : Convert.ToInt32(reader.GetValue(6)),
                H3B = reader.IsDBNull(7) ? 0 : Convert.ToInt32(reader.GetValue(7)),
                H4B = reader.IsDBNull(8) ? 0 : Convert.ToInt32(reader.GetValue(8)),
                HR = reader.IsDBNull(9) ? 0 : Convert.ToInt32(reader.GetValue(9)),
                FC = reader.IsDBNull(10) ? 0 : Convert.ToInt32(reader.GetValue(10)),
                BB = reader.IsDBNull(11) ? 0 : Convert.ToInt32(reader.GetValue(11)),
                SF = reader.IsDBNull(12) ? 0 : Convert.ToInt32(reader.GetValue(12)),
                K = reader.IsDBNull(13) ? 0 : Convert.ToInt32(reader.GetValue(13)),
                KF = reader.IsDBNull(14) ? 0 : Convert.ToInt32(reader.GetValue(14)),
                B = reader.IsDBNull(15) ? 0 : Convert.ToInt32(reader.GetValue(15)),
                GO = reader.IsDBNull(16) ? 0 : Convert.ToInt32(reader.GetValue(16)),
                FO = reader.IsDBNull(17) ? 0 : Convert.ToInt32(reader.GetValue(17)),
                O = reader.IsDBNull(18) ? 0 : Convert.ToInt32(reader.GetValue(18)),
                R = reader.IsDBNull(19) ? 0 : Convert.ToInt32(reader.GetValue(19)),
                RBI = reader.IsDBNull(20) ? 0 : Convert.ToInt32(reader.GetValue(20))
            });
        }
        return summaries;
    }

    public PlayerStatsSummary GetTeamTotals()
    {
        using SqliteConnection connection = new SqliteConnection(ConnectionString);
        connection.Open();

        SqliteCommand command = connection.CreateCommand();
        command.CommandText = 
        @"
            SELECT 
                'TEAM' as PlayerName,
                COUNT(DISTINCT GameId) as GamesPlayed,
                SUM(PA) as PA,
                SUM(AB) as AB,
                SUM(H) as H,
                SUM(H1B) as H1B,
                SUM(H2B) as H2B,
                SUM(H3B) as H3B,
                SUM(H4B) as H4B,
                SUM(HR) as HR,
                SUM(FC) as FC,
                SUM(BB) as BB,
                SUM(SF) as SF,
                SUM(K) as K,
                SUM(KF) as KF,
                SUM(B) as B,
                SUM(GO) as GO,
                SUM(FO) as FO,
                SUM(O) as O,
                SUM(R) as R,
                SUM(RBI) as RBI
            FROM Stats
        ";

        using SqliteDataReader reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new PlayerStatsSummary
            {
                PlayerName = reader.GetString(0),
                GamesPlayed = reader.IsDBNull(1) ? 0 : Convert.ToInt32(reader.GetValue(1)),
                PA = reader.IsDBNull(2) ? 0 : Convert.ToInt32(reader.GetValue(2)),
                AB = reader.IsDBNull(3) ? 0 : Convert.ToInt32(reader.GetValue(3)),
                H = reader.IsDBNull(4) ? 0 : Convert.ToInt32(reader.GetValue(4)),
                H1B = reader.IsDBNull(5) ? 0 : Convert.ToInt32(reader.GetValue(5)),
                H2B = reader.IsDBNull(6) ? 0 : Convert.ToInt32(reader.GetValue(6)),
                H3B = reader.IsDBNull(7) ? 0 : Convert.ToInt32(reader.GetValue(7)),
                H4B = reader.IsDBNull(8) ? 0 : Convert.ToInt32(reader.GetValue(8)),
                HR = reader.IsDBNull(9) ? 0 : Convert.ToInt32(reader.GetValue(9)),
                FC = reader.IsDBNull(10) ? 0 : Convert.ToInt32(reader.GetValue(10)),
                BB = reader.IsDBNull(11) ? 0 : Convert.ToInt32(reader.GetValue(11)),
                SF = reader.IsDBNull(12) ? 0 : Convert.ToInt32(reader.GetValue(12)),
                K = reader.IsDBNull(13) ? 0 : Convert.ToInt32(reader.GetValue(13)),
                KF = reader.IsDBNull(14) ? 0 : Convert.ToInt32(reader.GetValue(14)),
                B = reader.IsDBNull(15) ? 0 : Convert.ToInt32(reader.GetValue(15)),
                GO = reader.IsDBNull(16) ? 0 : Convert.ToInt32(reader.GetValue(16)),
                FO = reader.IsDBNull(17) ? 0 : Convert.ToInt32(reader.GetValue(17)),
                O = reader.IsDBNull(18) ? 0 : Convert.ToInt32(reader.GetValue(18)),
                R = reader.IsDBNull(19) ? 0 : Convert.ToInt32(reader.GetValue(19)),
                RBI = reader.IsDBNull(20) ? 0 : Convert.ToInt32(reader.GetValue(20))
            };
        }
        return new PlayerStatsSummary();
    }

    public List<Player> GetPlayers()
    {
        List<Player> players = new List<Player>();
        using SqliteConnection connection = new SqliteConnection(ConnectionString);
        connection.Open();

        SqliteCommand command = connection.CreateCommand();
        command.CommandText = "SELECT Id, Name, IsActive FROM Players ORDER BY Name";

        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            players.Add(new Player
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                IsActive = reader.GetInt32(2) != 0
            });
        }
        return players;
    }

    public void AddGameWithStats(Game game, List<StatEntry> stats)
    {
        using SqliteConnection connection = new SqliteConnection(ConnectionString);
        connection.Open();
        using SqliteTransaction transaction = connection.BeginTransaction();

        try
        {
            SqliteCommand gameCommand = connection.CreateCommand();
            gameCommand.Transaction = transaction;
            gameCommand.CommandText = "INSERT INTO Games (Date, Opponent, Type) VALUES ($date, $opponent, $type); SELECT last_insert_rowid();";
            gameCommand.Parameters.AddWithValue("$date", game.Date.ToString("yyyy-MM-dd"));
            gameCommand.Parameters.AddWithValue("$opponent", game.Opponent);
            gameCommand.Parameters.AddWithValue("$type", (int)game.Type);
            int gameId = Convert.ToInt32(gameCommand.ExecuteScalar());

            foreach (StatEntry stat in stats)
            {
                if (stat.PA == 0 && stat.AB == 0) continue; 

                SqliteCommand statCommand = connection.CreateCommand();
                statCommand.Transaction = transaction;
                statCommand.CommandText = 
                @"
                    INSERT INTO Stats (PlayerId, GameId, PA, AB, H, H1B, H2B, H3B, H4B, HR, FC, BB, SF, K, KF, B, GO, FO, O, R, RBI) 
                    VALUES ($playerId, $gameId, $pa, $ab, $h, $h1b, $h2b, $h3b, $h4b, $hr, $fc, $bb, $sf, $k, $kf, $b, $go, $fo, $o, $r, $rbi)
                ";
                statCommand.Parameters.AddWithValue("$playerId", stat.PlayerId);
                statCommand.Parameters.AddWithValue("$gameId", gameId);
                statCommand.Parameters.AddWithValue("$pa", stat.PA);
                statCommand.Parameters.AddWithValue("$ab", stat.AB);
                statCommand.Parameters.AddWithValue("$h", stat.H);
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
                statCommand.Parameters.AddWithValue("$b", stat.B);
                statCommand.Parameters.AddWithValue("$go", stat.GO);
                statCommand.Parameters.AddWithValue("$fo", stat.FO);
                statCommand.Parameters.AddWithValue("$o", stat.O);
                statCommand.Parameters.AddWithValue("$r", stat.R);
                statCommand.Parameters.AddWithValue("$rbi", stat.RBI);
                statCommand.ExecuteNonQuery();
            }

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }
}
