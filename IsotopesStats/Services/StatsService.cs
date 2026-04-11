using IsotopesStats.Models;
using Supabase;
using Postgrest;

namespace IsotopesStats.Services;

public class StatsService
{
    private readonly Client _supabase;

    public StatsService(Client supabase)
    {
        _supabase = supabase;
    }

    // --- SEASONS ---
    public async Task<List<Season>> GetSeasonsAsync()
    {
        BaseResponse<Season> response = await _supabase.From<Season>()
            .Where(x => x.IsDeleted == false)
            .Order("name", Constants.Ordering.Descending)
            .Get();
        return response.Models;
    }

    public async Task<int> AddSeasonAsync(Season season)
    {
        BaseResponse<Season> response = await _supabase.From<Season>().Insert(season);
        return response.Model?.Id ?? 0;
    }

    public async Task UpdateSeasonAsync(Season season)
    {
        await _supabase.From<Season>().Update(season);
    }

    public async Task DeleteSeasonAsync(int seasonId)
    {
        Season season = new Season { Id = seasonId, IsDeleted = true };
        await _supabase.From<Season>().Update(season);
    }

    // --- PLAYERS ---
    public async Task<List<Player>> GetAllPlayersAsync()
    {
        BaseResponse<Player> response = await _supabase.From<Player>()
            .Where(x => x.IsDeleted == false)
            .Order("name", Constants.Ordering.Ascending)
            .Get();
        return response.Models;
    }

    public async Task<List<Player>> GetPlayersAsync(int seasonId)
    {
        BaseResponse<Player> response = await _supabase.From<Player>()
            .Join<SeasonPlayers>("id", "playerid")
            .Where<SeasonPlayers>(x => x.SeasonId == seasonId)
            .Where(x => x.IsDeleted == false)
            .Order("name", Constants.Ordering.Ascending)
            .Get();
        return response.Models;
    }

    public async Task<int> AddPlayerAsync(Player player, int seasonId)
    {
        BaseResponse<Player> response = await _supabase.From<Player>().Insert(player);
        int playerId = response.Model?.Id ?? 0;

        if (playerId != 0 && seasonId != 0)
        {
            await AddPlayerToSeasonAsync(playerId, seasonId);
        }
        return playerId;
    }

    public async Task UpdatePlayerAsync(Player player)
    {
        await _supabase.From<Player>().Update(player);
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
            .Where(x => x.PlayerId == playerId)
            .Where(x => x.SeasonId == seasonId)
            .Delete();
    }

    // --- OPPONENTS ---
    public async Task<List<Opponent>> GetAllOpponentsAsync()
    {
        BaseResponse<Opponent> response = await _supabase.From<Opponent>()
            .Where(x => x.IsDeleted == false)
            .Order("name", Constants.Ordering.Ascending)
            .Get();
        return response.Models;
    }

    public async Task<int> AddOpponentAsync(Opponent opponent, int seasonId)
    {
        BaseResponse<Opponent> response = await _supabase.From<Opponent>().Insert(opponent);
        return response.Model?.Id ?? 0;
    }

    public async Task UpdateOpponentAsync(Opponent opponent)
    {
        await _supabase.From<Opponent>().Update(opponent);
    }

    public async Task DeleteOpponentAsync(int opponentId)
    {
        Opponent opponent = new Opponent { Id = opponentId, IsDeleted = true };
        await _supabase.From<Opponent>().Update(opponent);
    }

    // --- GAMES & STATS ---
    public async Task<Game?> GetGameAsync(int gameId)
    {
        BaseResponse<Game> response = await _supabase.From<Game>("v_games_management")
            .Where("id", Constants.Operator.Equals, gameId)
            .Single();
        return response.Model;
    }

    public async Task<List<PlayerStatsSummary>> GetStatsSummaryAsync(int seasonId)
    {
        BaseResponse<PlayerStatsSummary> response = await _supabase.From<PlayerStatsSummary>("v_player_stats_summary")
            .Where("seasonid", Constants.Operator.Equals, seasonId)
            .Get();
        return response.Models;
    }

    public async Task<PlayerStatsSummary> GetTeamTotalsAsync(int seasonId)
    {
        BaseResponse<PlayerStatsSummary> response = await _supabase.From<PlayerStatsSummary>("v_team_stats_summary")
            .Where("seasonid", Constants.Operator.Equals, seasonId)
            .Single();
        return response.Model ?? new PlayerStatsSummary { PlayerName = "TEAM TOTALS" };
    }

    public async Task<List<StatEntry>> GetAllGameStatsAsync(int seasonId)
    {
        BaseResponse<StatEntry> response = await _supabase.From<StatEntry>("v_game_stats_extended")
            .Where("seasonid", Constants.Operator.Equals, seasonId)
            .Where("gameisdeleted", Constants.Operator.Equals, false)
            .Order("date", Constants.Ordering.Descending)
            .Order("bo", Constants.Ordering.Ascending)
            .Get();
        return response.Models;
    }

    public async Task AddGameWithStatsAsync(Game game, List<StatEntry> stats)
    {
        BaseResponse<Game> gameResponse = await _supabase.From<Game>().Insert(game);
        int gameId = gameResponse.Model?.Id ?? 0;

        if (gameId != 0)
        {
            foreach (StatEntry stat in stats)
            {
                stat.GameId = gameId;
            }
            await _supabase.From<StatEntry>().Insert(stats);
        }
    }

    public async Task DeleteGameAsync(int gameId)
    {
        Game game = new Game { Id = gameId, IsDeleted = true };
        await _supabase.From<Game>().Update(game);
    }
}
