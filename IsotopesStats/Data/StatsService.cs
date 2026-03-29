using Microsoft.Data.Sqlite;
using IsotopesStats.Models;

namespace IsotopesStats.Data;

public class StatsService
{
    private const string ConnectionString = "Data Source=IsotopesStats.db";

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
                SUM(s.GO) as GO,
                SUM(s.FO) as FO,
                SUM(s.R) as R,
                SUM(s.RBI) as RBI
            FROM Players p
            LEFT JOIN Stats s ON p.Id = s.PlayerId
            GROUP BY p.Id, p.Name
            ORDER BY (CAST(SUM(s.H1B + s.H2B + s.H3B + s.H4B + s.HR) AS FLOAT) / NULLIF(SUM(s.H1B + s.H2B + s.H3B + s.H4B + s.HR + s.FC + s.K + s.KF + s.GO + s.FO), 0)) DESC
        ";

        using SqliteDataReader reader = command.ExecuteReader();
        while (reader.Read())
        {
            summaries.Add(new PlayerStatsSummary
            {
                PlayerName = reader.GetString(0),
                GamesPlayed = Convert.ToInt32(reader.GetValue(1)),
                H1B = reader.IsDBNull(2) ? 0 : Convert.ToInt32(reader.GetValue(2)),
                H2B = reader.IsDBNull(3) ? 0 : Convert.ToInt32(reader.GetValue(3)),
                H3B = reader.IsDBNull(4) ? 0 : Convert.ToInt32(reader.GetValue(4)),
                H4B = reader.IsDBNull(5) ? 0 : Convert.ToInt32(reader.GetValue(5)),
                HR = reader.IsDBNull(6) ? 0 : Convert.ToInt32(reader.GetValue(6)),
                FC = reader.IsDBNull(7) ? 0 : Convert.ToInt32(reader.GetValue(7)),
                BB = reader.IsDBNull(8) ? 0 : Convert.ToInt32(reader.GetValue(8)),
                SF = reader.IsDBNull(9) ? 0 : Convert.ToInt32(reader.GetValue(9)),
                K = reader.IsDBNull(10) ? 0 : Convert.ToInt32(reader.GetValue(10)),
                KF = reader.IsDBNull(11) ? 0 : Convert.ToInt32(reader.GetValue(11)),
                GO = reader.IsDBNull(12) ? 0 : Convert.ToInt32(reader.GetValue(12)),
                FO = reader.IsDBNull(13) ? 0 : Convert.ToInt32(reader.GetValue(13)),
                R = reader.IsDBNull(14) ? 0 : Convert.ToInt32(reader.GetValue(14)),
                RBI = reader.IsDBNull(15) ? 0 : Convert.ToInt32(reader.GetValue(15))
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
                SUM(GO) as GO,
                SUM(FO) as FO,
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
                H1B = reader.IsDBNull(2) ? 0 : Convert.ToInt32(reader.GetValue(2)),
                H2B = reader.IsDBNull(3) ? 0 : Convert.ToInt32(reader.GetValue(3)),
                H3B = reader.IsDBNull(4) ? 0 : Convert.ToInt32(reader.GetValue(4)),
                H4B = reader.IsDBNull(5) ? 0 : Convert.ToInt32(reader.GetValue(5)),
                HR = reader.IsDBNull(6) ? 0 : Convert.ToInt32(reader.GetValue(6)),
                FC = reader.IsDBNull(7) ? 0 : Convert.ToInt32(reader.GetValue(7)),
                BB = reader.IsDBNull(8) ? 0 : Convert.ToInt32(reader.GetValue(8)),
                SF = reader.IsDBNull(9) ? 0 : Convert.ToInt32(reader.GetValue(9)),
                K = reader.IsDBNull(10) ? 0 : Convert.ToInt32(reader.GetValue(10)),
                KF = reader.IsDBNull(11) ? 0 : Convert.ToInt32(reader.GetValue(11)),
                GO = reader.IsDBNull(12) ? 0 : Convert.ToInt32(reader.GetValue(12)),
                FO = reader.IsDBNull(13) ? 0 : Convert.ToInt32(reader.GetValue(13)),
                R = reader.IsDBNull(14) ? 0 : Convert.ToInt32(reader.GetValue(14)),
                RBI = reader.IsDBNull(15) ? 0 : Convert.ToInt32(reader.GetValue(15))
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
            gameCommand.CommandText = "INSERT INTO Games (GameNumber, Date, Time, Diamond, Opponent, Type) VALUES ($gameNumber, $date, $time, $diamond, $opponent, $type); SELECT last_insert_rowid();";
            gameCommand.Parameters.AddWithValue("$gameNumber", game.GameNumber);
            gameCommand.Parameters.AddWithValue("$date", game.Date.ToString("yyyy-MM-dd"));
            gameCommand.Parameters.AddWithValue("$time", game.Time);
            gameCommand.Parameters.AddWithValue("$diamond", game.Diamond);
            gameCommand.Parameters.AddWithValue("$opponent", game.Opponent);
            gameCommand.Parameters.AddWithValue("$type", (int)game.Type);
            int gameId = Convert.ToInt32(gameCommand.ExecuteScalar());

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
