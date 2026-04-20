using IsotopesStats.Models;
using Supabase;
using Postgrest;
using Postgrest.Responses;

namespace IsotopesStats.Services;

public class StatsService
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
        // Create a clean object to avoid Postgrest tracking issues with deserialized models
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
        ModeledResponse<Player> response = await _supabase.From<Player>().Insert(player);
        int playerId = response.Model?.Id ?? 0;
        if (playerId != 0 && seasonId != 0)
        {
            await AddPlayerToSeasonAsync(playerId, seasonId);
        }
        return playerId;
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
        ModeledResponse<Opponent> response = await _supabase.From<Opponent>()
            .Where(x => x.IsDeleted == false)
            .Order("name", Constants.Ordering.Ascending)
            .Get();
        return response.Models;
    }

    public async Task<int> AddOpponentAsync(Opponent opponent, int seasonId)
    {
        ModeledResponse<Opponent> response = await _supabase.From<Opponent>().Insert(opponent);
        int opponentId = response.Model?.Id ?? 0;
        if (opponentId != 0 && seasonId != 0)
        {
            await AddOpponentToSeasonAsync(opponentId, seasonId);
        }
        return opponentId;
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

    public async Task AddOpponentToSeasonAsync(int opponentId, int seasonId)
    {
        SeasonOpponents link = new SeasonOpponents { SeasonId = seasonId, OpponentId = opponentId };
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

    public async Task<List<StatEntry>> GetGameStatsAsync(int gameId)
    {
        ModeledResponse<StatEntry> response = await _supabase.From<StatEntry>()
            .Select("*, player:players(*)")
            .Where(x => x.GameId == gameId)
            .Order("bo", Constants.Ordering.Ascending)
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

    public async Task<List<GameSummaryView>> GetGameSummariesAsync(int seasonId)
    {
        ModeledResponse<GameSummaryView> response = await _supabase.From<GameSummaryView>()
            .Filter("seasonid", Constants.Operator.Equals, seasonId)
            .Order("date", Constants.Ordering.Descending)
            .Get();
        return response.Models;
    }

    public async Task<List<PlayerStatsSummary>> GetStatsSummaryAsync(int seasonId)
    {
        ModeledResponse<PlayerStatsSummary> response = await _supabase.From<PlayerStatsSummary>()
            .Filter("seasonid", Constants.Operator.Equals, seasonId)
            .Get();
        return response.Models;
    }

    public async Task<PlayerStatsSummary> GetTeamTotalsAsync(int seasonId)
    {
        TeamStatsSummary? result = await _supabase.From<TeamStatsSummary>()
            .Filter("seasonid", Constants.Operator.Equals, seasonId)
            .Single();
        return (PlayerStatsSummary?)result ?? new TeamStatsSummary { PlayerName = "TEAM TOTALS" };
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

    public async Task AddGameWithStatsAsync(Game game, List<StatEntry> stats)
    {
        ModeledResponse<Game> gameResponse = await _supabase.From<Game>().Insert(game);
        int gameId = gameResponse.Model?.Id ?? 0;
        if (gameId != 0)
        {
            foreach (StatEntry stat in stats) { stat.GameId = gameId; }
            await _supabase.From<StatEntry>().Insert(stats);
        }
    }

    public async Task UpdateGameWithStatsAsync(Game game, List<StatEntry> stats)
    {
        Game update = new Game 
        { 
            Id = game.Id, 
            SeasonId = game.SeasonId, 
            GameNumber = game.GameNumber, 
            Date = game.Date, 
            Diamond = game.Diamond, 
            OpponentId = game.OpponentId, 
            Type = game.Type, 
            IsDeleted = game.IsDeleted 
        };
        await _supabase.From<Game>().Update(update);
        await _supabase.From<StatEntry>().Where(x => x.GameId == game.Id).Delete();
        foreach (StatEntry stat in stats) { stat.GameId = game.Id; stat.Id = 0; }
        await _supabase.From<StatEntry>().Insert(stats);
    }

    public async Task DeleteGameAsync(int gameId)
    {
        Game game = new Game { Id = gameId, IsDeleted = true };
        await _supabase.From<Game>().Update(game);
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

    public async Task<List<Season>> GetSeasonsForOpponentAsync(int opponentId)
    {
        return await GetSeasonsAsync();
    }

    public async Task<List<Season>> GetSeasonsForPlayerAsync(int playerId)
    {
        return await GetSeasonsAsync();
    }
}

