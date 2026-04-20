using IsotopesStats.Models;
using Supabase;
using Postgrest;
using Postgrest.Responses;
using Newtonsoft.Json;

namespace IsotopesStats.Services;

public class StatsService : IStatsService
{
    private readonly Supabase.Client _supabase;

    public StatsService(Supabase.Client supabase)
    {
        _supabase = supabase;
    }

    // --- SEASONS ---
    public async Task<List<Season>> GetSeasonsAsync()
    {
        ModeledResponse<Season> response = await _supabase.From<Season>()
            .Where(x => x.IsDeleted == false)
            .Order("name", Constants.Ordering.Descending)
            .Get();
        return response.Models;
    }

    public async Task<int> AddSeasonAsync(Season season)
    {
        ModeledResponse<Season> response = await _supabase.From<Season>().Insert(season);
        return response.Model?.Id ?? 0;
    }

    public async Task UpdateSeasonAsync(Season season)
    {
        Season update = new Season { Id = season.Id, Name = season.Name, IsDeleted = season.IsDeleted };
        await _supabase.From<Season>().Update(update);
    }

    public async Task DeleteSeasonAsync(int seasonId)
    {
        Season season = new Season { Id = seasonId, IsDeleted = true };
        await _supabase.From<Season>().Update(season);
    }

    public async Task<bool> IsSeasonNameUniqueAsync(string name, int excludeId = 0)
    {
        ModeledResponse<Season> response = await _supabase.From<Season>()
            .Where(x => x.Name == name)
            .Where(x => x.Id != excludeId)
            .Where(x => x.IsDeleted == false)
            .Get();
        return response.Models.Count == 0;
    }

    // --- PLAYERS ---
    public async Task<List<Player>> GetAllPlayersAsync()
    {
        ModeledResponse<Player> response = await _supabase.From<Player>()
            .Where(x => x.IsDeleted == false)
            .Order("name", Constants.Ordering.Ascending)
            .Get();
        return response.Models;
    }

    public async Task<List<Player>> GetPlayersAsync(int seasonId)
    {
        ModeledResponse<SeasonPlayerView> response = await _supabase.From<SeasonPlayerView>()
            .Filter("seasonid", Constants.Operator.Equals, seasonId)
            .Get();
        
        List<Player> players = new List<Player>();
        foreach (SeasonPlayerView sp in response.Models)
        {
            players.Add(new Player { Id = sp.PlayerId, Name = sp.PlayerName });
        }
        return players;
    }

    public async Task<int> AddPlayerAsync(Player player, int seasonId)
    {
        if (seasonId == 0)
        {
            ModeledResponse<Player> insertResponse = await _supabase.From<Player>().Insert(player);
            return insertResponse.Model?.Id ?? 0;
        }

        Dictionary<string, object> parameters = new Dictionary<string, object>
        {
            { "p_name", player.Name },
            { "p_season_id", seasonId }
        };

        BaseResponse rpcResponse = await _supabase.Rpc("add_player_to_season", parameters);
        return string.IsNullOrEmpty(rpcResponse.Content) ? 0 : JsonConvert.DeserializeObject<int>(rpcResponse.Content);
    }

    public async Task UpdatePlayerAsync(Player player)
    {
        Player update = new Player { Id = player.Id, Name = player.Name, IsDeleted = player.IsDeleted };
        await _supabase.From<Player>().Update(update);
    }

    public async Task DeletePlayerAsync(int playerId)
    {
        Player player = new Player { Id = playerId, IsDeleted = true };
        await _supabase.From<Player>().Update(player);
    }

    public async Task AddPlayerToSeasonAsync(int playerId, int seasonId)
    {
        SeasonPlayers link = new SeasonPlayers { PlayerId = playerId, SeasonId = seasonId };
        await _supabase.From<SeasonPlayers>().Insert(link);
    }

    public async Task DeletePlayerFromSeasonAsync(int playerId, int seasonId)
    {
        await _supabase.From<SeasonPlayers>()
            .Filter("playerid", Constants.Operator.Equals, playerId)
            .Filter("seasonid", Constants.Operator.Equals, seasonId)
            .Delete();
    }

    public async Task<bool> IsPlayerNameUniqueAsync(string name, int excludeId = 0)
    {
        ModeledResponse<Player> response = await _supabase.From<Player>()
            .Where(x => x.Name == name)
            .Where(x => x.Id != excludeId)
            .Where(x => x.IsDeleted == false)
            .Get();
        return response.Models.Count == 0;
    }

    public async Task<List<Season>> GetSeasonsForPlayerAsync(int playerId)
    {
        return await GetSeasonsAsync();
    }

    // --- OPPONENTS ---
    public async Task<List<Opponent>> GetAllOpponentsAsync()
    {
        ModeledResponse<Opponent> response = await _supabase.From<Opponent>()
            .Where(x => x.IsDeleted == false)
            .Order("name", Constants.Ordering.Ascending)
            .Get();
        return response.Models;
    }

    public async Task<List<Opponent>> GetOpponentsAsync(int seasonId)
    {
        ModeledResponse<SeasonOpponentView> response = await _supabase.From<SeasonOpponentView>()
            .Filter("seasonid", Constants.Operator.Equals, seasonId)
            .Get();
        
        List<Opponent> opponents = new List<Opponent>();
        foreach (SeasonOpponentView sov in response.Models)
        {
            opponents.Add(new Opponent { 
                Id = sov.OpponentId, 
                Name = sov.OpponentName 
            });
        }
        return opponents.OrderBy(o => o.Name).ToList();
    }

    public async Task<int> AddOpponentAsync(Opponent opponent, int seasonId)
    {
        if (seasonId == 0)
        {
            ModeledResponse<Opponent> insertResponse = await _supabase.From<Opponent>().Insert(opponent);
            return insertResponse.Model?.Id ?? 0;
        }

        Dictionary<string, object> parameters = new Dictionary<string, object>
        {
            { "p_name", opponent.Name },
            { "p_season_id", seasonId }
        };

        BaseResponse rpcResponse = await _supabase.Rpc("add_opponent_to_season", parameters);
        return string.IsNullOrEmpty(rpcResponse.Content) ? 0 : JsonConvert.DeserializeObject<int>(rpcResponse.Content);
    }

    public async Task UpdateOpponentAsync(Opponent opponent)
    {
        Opponent update = new Opponent { Id = opponent.Id, Name = opponent.Name, IsDeleted = opponent.IsDeleted };
        await _supabase.From<Opponent>().Update(update);
    }

    public async Task DeleteOpponentAsync(int opponentId)
    {
        Opponent opponent = new Opponent { Id = opponentId, IsDeleted = true };
        await _supabase.From<Opponent>().Update(opponent);
    }

    public async Task<bool> IsOpponentNameUniqueAsync(string name, int excludeId = 0)
    {
        ModeledResponse<Opponent> response = await _supabase.From<Opponent>()
            .Where(x => x.Name == name)
            .Where(x => x.Id != excludeId)
            .Where(x => x.IsDeleted == false)
            .Get();
        return response.Models.Count == 0;
    }

    public async Task<List<SeasonOpponents>> GetSeasonsForOpponentAsync(int opponentId)
    {
        ModeledResponse<SeasonOpponents> response = await _supabase.From<SeasonOpponents>()
            .Filter("opponentid", Constants.Operator.Equals, opponentId)
            .Get();
        return response.Models;
    }

    public async Task AddOpponentToSeasonAsync(int opponentId, int seasonId, string? name = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            Opponent? master = await _supabase.From<Opponent>().Where(x => x.Id == opponentId).Single();
            name = master?.Name;
        }
        SeasonOpponents link = new SeasonOpponents { SeasonId = seasonId, OpponentId = opponentId, Name = name };
        await _supabase.From<SeasonOpponents>().Insert(link);
    }

    public async Task DeleteOpponentFromSeasonAsync(int opponentId, int seasonId)
    {
        await _supabase.From<SeasonOpponents>()
            .Filter("opponentid", Constants.Operator.Equals, opponentId)
            .Filter("seasonid", Constants.Operator.Equals, seasonId)
            .Delete();
    }

    // --- GAMES & STATS ---
    public async Task<Game?> GetGameAsync(int gameId)
    {
        return await _supabase.From<Game>()
            .Filter("id", Constants.Operator.Equals, gameId)
            .Single();
    }

    public async Task<List<Game>> GetGamesBySeasonAsync(int seasonId)
    {
        ModeledResponse<GameManagementView> response = await _supabase.From<GameManagementView>()
            .Filter("seasonid", Constants.Operator.Equals, seasonId)
            .Order("date", Constants.Ordering.Descending)
            .Get();
        
        List<Game> games = new List<Game>();
        foreach (GameManagementView v in response.Models)
        {
            games.Add(new Game {
                Id = v.Id,
                SeasonId = v.SeasonId,
                GameNumber = v.GameNumber,
                Date = v.Date,
                Diamond = v.Diamond,
                IsHome = v.IsHome,
                OpponentId = v.OpponentId,
                Type = v.GameType,
                IsDeleted = v.IsDeleted,
                Opponent = new Opponent { Id = v.OpponentId, Name = v.OpponentName }
            });
        }
        return games;
    }

    public async Task<List<StatEntry>> GetGameStatsAsync(int gameId)
    {
        ModeledResponse<StatEntry> response = await _supabase.From<StatEntry>()
            .Select("*, Player:players(*)")
            .Where(x => x.GameId == gameId)
            .Order("bo", Constants.Ordering.Ascending)
            .Get();
        return response.Models;
    }

    public async Task<List<PlayerStatsSummary>> GetStatsSummaryAsync(int seasonId)
    {
        if (seasonId == -1) return await GetAllStatsSummaryAsync();
        
        ModeledResponse<PlayerStatsSummary> response = await _supabase.From<PlayerStatsSummary>()
            .Filter("seasonid", Constants.Operator.Equals, seasonId)
            .Get();
        return response.Models;
    }

    private async Task<List<PlayerStatsSummary>> GetAllStatsSummaryAsync()
    {
        ModeledResponse<PlayerStatsSummary> response = await _supabase.From<PlayerStatsSummary>().Get();
        
        return response.Models.GroupBy(p => p.PlayerName)
            .Select(g => new PlayerStatsSummary
            {
                PlayerName = g.Key,
                GamesPlayed = g.Sum(x => x.GamesPlayed),
                H1B = g.Sum(x => x.H1B),
                H2B = g.Sum(x => x.H2B),
                H3B = g.Sum(x => x.H3B),
                H4B = g.Sum(x => x.H4B),
                HR = g.Sum(x => x.HR),
                FC = g.Sum(x => x.FC),
                BB = g.Sum(x => x.BB),
                SF = g.Sum(x => x.SF),
                K = g.Sum(x => x.K),
                KF = g.Sum(x => x.KF),
                GO = g.Sum(x => x.GO),
                FO = g.Sum(x => x.FO),
                R = g.Sum(x => x.R),
                RBI = g.Sum(x => x.RBI)
            }).ToList();
    }

    public async Task<PlayerStatsSummary> GetTeamTotalsAsync(int seasonId)
    {
        if (seasonId == -1) return await GetAllTeamTotalsAsync();

        TeamStatsSummary? result = await _supabase.From<TeamStatsSummary>()
            .Filter("seasonid", Constants.Operator.Equals, seasonId)
            .Single();
        return (PlayerStatsSummary?)result ?? new TeamStatsSummary { PlayerName = "TEAM TOTALS" };
    }

    private async Task<PlayerStatsSummary> GetAllTeamTotalsAsync()
    {
        ModeledResponse<TeamStatsSummary> response = await _supabase.From<TeamStatsSummary>().Get();
        
        return new PlayerStatsSummary
        {
            PlayerName = "TEAM TOTALS",
            GamesPlayed = response.Models.Sum(x => x.GamesPlayed),
            H1B = response.Models.Sum(x => x.H1B),
            H2B = response.Models.Sum(x => x.H2B),
            H3B = response.Models.Sum(x => x.H3B),
            H4B = response.Models.Sum(x => x.H4B),
            HR = response.Models.Sum(x => x.HR),
            FC = response.Models.Sum(x => x.FC),
            BB = response.Models.Sum(x => x.BB),
            SF = response.Models.Sum(x => x.SF),
            K = response.Models.Sum(x => x.K),
            KF = response.Models.Sum(x => x.KF),
            GO = response.Models.Sum(x => x.GO),
            FO = response.Models.Sum(x => x.FO),
            R = response.Models.Sum(x => x.R),
            RBI = response.Models.Sum(x => x.RBI)
        };
    }

    public async Task<int> GetMostRecentStatsSeasonIdAsync()
    {
        ModeledResponse<GameStatsExtendedView> response = await _supabase.From<GameStatsExtendedView>()
            .Order("gameid", Constants.Ordering.Descending)
            .Limit(1)
            .Get();
        
        return response.Model?.SeasonId ?? 0;
    }

    public async Task AddGameWithStatsAsync(Game game, List<StatEntry> stats)
    {
        Dictionary<string, object> parameters = new Dictionary<string, object>
        {
            { "p_season_id", game.SeasonId },
            { "p_game_number", game.GameNumber },
            { "p_date", game.Date },
            { "p_diamond", game.Diamond },
            { "p_ishome", game.IsHome },
            { "p_opponent_id", game.OpponentId },
            { "p_type", (int)game.Type },
            { "p_stats", stats }
        };

        await _supabase.Rpc("create_game_with_stats", parameters);
    }

    public async Task UpdateGameWithStatsAsync(Game game, List<StatEntry> stats)
    {
        Dictionary<string, object> parameters = new Dictionary<string, object>
        {
            { "p_game_id", game.Id },
            { "p_season_id", game.SeasonId },
            { "p_game_number", game.GameNumber },
            { "p_date", game.Date },
            { "p_diamond", game.Diamond },
            { "p_ishome", game.IsHome },
            { "p_opponent_id", game.OpponentId },
            { "p_type", (int)game.Type },
            { "p_is_deleted", game.IsDeleted },
            { "p_stats", stats }
        };

        await _supabase.Rpc("update_game_with_stats", parameters);
    }

    public async Task DeleteGameAsync(int gameId)
    {
        Game game = new Game { Id = gameId, IsDeleted = true };
        await _supabase.From<Game>().Update(game);
    }

    public async Task<List<GameStatsExtendedView>> GetAllGameStatsAsync(int seasonId)
    {
        ModeledResponse<GameStatsExtendedView> response = await _supabase.From<GameStatsExtendedView>()
            .Filter("seasonid", Constants.Operator.Equals, seasonId)
            .Where(x => x.GameIsDeleted == false)
            .Order("date", Constants.Ordering.Descending)
            .Order("bo", Constants.Ordering.Ascending)
            .Get();
        return response.Models;
    }

    public async Task<List<GameStatsExtendedView>> GetPlayerGameLogAsync(string playerName, int seasonId)
    {
        ModeledResponse<GameStatsExtendedView> response = await _supabase.From<GameStatsExtendedView>()
            .Filter("playername", Constants.Operator.Equals, playerName)
            .Filter("seasonid", Constants.Operator.Equals, seasonId)
            .Where(x => x.GameIsDeleted == false)
            .Order("date", Constants.Ordering.Descending)
            .Get();
        return response.Models;
    }

    public async Task<int> GetNextGameNumberAsync(int seasonId)
    {
        ModeledResponse<Game> response = await _supabase.From<Game>()
            .Filter("seasonid", Constants.Operator.Equals, seasonId)
            .Where(x => x.IsDeleted == false)
            .Order("gamenumber", Constants.Ordering.Descending)
            .Limit(1)
            .Get();
        
        return (response.Model?.GameNumber ?? 0) + 1;
    }

    public async Task<List<GameSummaryView>> GetGameSummariesAsync(int seasonId)
    {
        ModeledResponse<GameSummaryView> response = await _supabase.From<GameSummaryView>()
            .Filter("seasonid", Constants.Operator.Equals, seasonId)
            .Order("date", Constants.Ordering.Descending)
            .Get();
        return response.Models;
    }

    public async Task<List<GameStatsExtendedView>> GetExtendedGameStatsAsync(int gameId)
    {
        ModeledResponse<GameStatsExtendedView> response = await _supabase.From<GameStatsExtendedView>()
            .Filter("gameid", Constants.Operator.Equals, gameId)
            .Order("bo", Constants.Ordering.Ascending)
            .Get();
        return response.Models;
    }
}
