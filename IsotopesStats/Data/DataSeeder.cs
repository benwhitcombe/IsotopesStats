using Microsoft.Data.Sqlite;
using IsotopesStats.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace IsotopesStats.Data;

public static class DataSeeder
{
    private const string ConnectionString = "Data Source=Data/IsotopesStats.db";

    private class RawGameData
    {
        [JsonPropertyName("Game #")]
        public int GameNumber { get; set; }
        public string Date { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;
        public string Diamond { get; set; } = string.Empty;
        public string Opposition { get; set; } = string.Empty;
        public string Player { get; set; } = string.Empty;
        public int BO { get; set; }
        [JsonPropertyName("1B")]
        public int H1B { get; set; }
        [JsonPropertyName("2B")]
        public int H2B { get; set; }
        [JsonPropertyName("3B")]
        public int H3B { get; set; }
        [JsonPropertyName("4B")]
        public int H4B { get; set; }
        public int HR { get; set; }
        public int FC { get; set; }
        public int BB { get; set; }
        public int SF { get; set; }
        public int K { get; set; }
        public int KF { get; set; }
        public int GO { get; set; }
        public int FO { get; set; }
        public int R { get; set; }
        public int RBI { get; set; }
    }

    public static void Seed2025Data()
    {
        using SqliteConnection connection = new SqliteConnection(ConnectionString);
        connection.Open();

        // Check if data already exists
        SqliteCommand checkCommand = connection.CreateCommand();
        checkCommand.CommandText = "SELECT COUNT(*) FROM Players";
        if (Convert.ToInt32(checkCommand.ExecuteScalar()) > 0) return;

        string jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data/2025_data.json");
        if (!File.Exists(jsonPath))
        {
            jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "Data/2025_data.json");
        }

        if (!File.Exists(jsonPath))
        {
            Console.WriteLine($"Warning: Could not find seed data file at {jsonPath}");
            return;
        }

        string jsonContent = File.ReadAllText(jsonPath);
        List<RawGameData>? allRows = JsonSerializer.Deserialize<List<RawGameData>>(jsonContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (allRows == null) return;

        using SqliteTransaction transaction = connection.BeginTransaction();
        try
        {
            Dictionary<string, int> playerIds = new Dictionary<string, int>();
            IEnumerable<IGrouping<dynamic, RawGameData>> games = allRows.GroupBy(r => new { r.GameNumber, r.Date, r.Opposition, r.Time, r.Diamond });

            foreach (IGrouping<dynamic, RawGameData> gameGroup in games)
            {
                SqliteCommand gameCommand = connection.CreateCommand();
                gameCommand.Transaction = transaction;
                gameCommand.CommandText = "INSERT INTO Games (GameNumber, Date, Diamond, Opponent, Type) VALUES ($gameNumber, $date, $diamond, $opponent, 0); SELECT last_insert_rowid();";
                gameCommand.Parameters.AddWithValue("$gameNumber", gameGroup.Key.GameNumber);
                
                string combinedDateTime = $"{gameGroup.Key.Date} {gameGroup.Key.Time?.ToString() ?? "00:00"}";
                gameCommand.Parameters.AddWithValue("$date", combinedDateTime);
                
                gameCommand.Parameters.AddWithValue("$diamond", gameGroup.Key.Diamond);
                gameCommand.Parameters.AddWithValue("$opponent", gameGroup.Key.Opposition);
                int gameId = Convert.ToInt32(gameCommand.ExecuteScalar());

                foreach (RawGameData row in gameGroup)
                {
                    if (!playerIds.ContainsKey(row.Player))
                    {
                        SqliteCommand playerCommand = connection.CreateCommand();
                        playerCommand.Transaction = transaction;
                        playerCommand.CommandText = "INSERT INTO Players (Name) VALUES ($name); SELECT last_insert_rowid();";
                        playerCommand.Parameters.AddWithValue("$name", row.Player);
                        playerIds[row.Player] = Convert.ToInt32(playerCommand.ExecuteScalar());
                    }

                    int playerId = playerIds[row.Player];

                    SqliteCommand statCommand = connection.CreateCommand();
                    statCommand.Transaction = transaction;
                    statCommand.CommandText = 
                    @"
                        INSERT INTO Stats (PlayerId, GameId, BO, H1B, H2B, H3B, H4B, HR, FC, BB, SF, K, KF, GO, FO, R, RBI) 
                        VALUES ($playerId, $gameId, $bo, $h1b, $h2b, $h3b, $h4b, $hr, $fc, $bb, $sf, $k, $kf, $go, $fo, $r, $rbi)
                    ";
                    statCommand.Parameters.AddWithValue("$playerId", playerId);
                    statCommand.Parameters.AddWithValue("$gameId", gameId);
                    statCommand.Parameters.AddWithValue("$bo", row.BO);
                    statCommand.Parameters.AddWithValue("$h1b", row.H1B);
                    statCommand.Parameters.AddWithValue("$h2b", row.H2B);
                    statCommand.Parameters.AddWithValue("$h3b", row.H3B);
                    statCommand.Parameters.AddWithValue("$h4b", row.H4B);
                    statCommand.Parameters.AddWithValue("$hr", row.HR);
                    statCommand.Parameters.AddWithValue("$fc", row.FC);
                    statCommand.Parameters.AddWithValue("$bb", row.BB);
                    statCommand.Parameters.AddWithValue("$sf", row.SF);
                    statCommand.Parameters.AddWithValue("$k", row.K);
                    statCommand.Parameters.AddWithValue("$kf", row.KF);
                    statCommand.Parameters.AddWithValue("$go", row.GO);
                    statCommand.Parameters.AddWithValue("$fo", row.FO);
                    statCommand.Parameters.AddWithValue("$r", row.R);
                    statCommand.Parameters.AddWithValue("$rbi", row.RBI);
                    statCommand.ExecuteNonQuery();
                }
            }
            transaction.Commit();
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            Console.WriteLine($"Error seeding data: {ex.Message}");
            throw;
        }
    }
}