using IsotopesStats.Models;
using IsotopesStats.Data;

namespace IsotopesStats.Services;

public class StatsService
{
    private readonly StatsRepository _repository;

    public StatsService(StatsRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Season>> GetSeasonsAsync()
    {
        return await _repository.GetSeasonsAsync();
    }

    public async Task<List<PlayerStatsSummary>> GetStatsSummaryAsync(int seasonId)
    {
        return await _repository.GetStatsSummaryAsync(seasonId);
    }

    public async Task<PlayerStatsSummary> GetTeamTotalsAsync(int seasonId)
    {
        return await _repository.GetTeamTotalsAsync(seasonId);
    }

    public async Task<List<Player>> GetPlayersAsync(int seasonId)
    {
        return await _repository.GetPlayersAsync(seasonId);
    }

    public async Task<List<StatEntry>> GetAllGameStatsAsync(int seasonId)
    {
        return await _repository.GetAllGameStatsAsync(seasonId);
    }

    public async Task<List<StatEntry>> GetPlayerGameLogAsync(string playerName, int seasonId)
    {
        return await _repository.GetPlayerGameLogAsync(playerName, seasonId);
    }

    public async Task AddGameWithStatsAsync(Game game, List<StatEntry> stats)
    {
        await _repository.AddGameWithStatsAsync(game, stats);
    }

    public async Task UpdateGameAsync(Game game)
    {
        await _repository.UpdateGameAsync(game);
    }

    public async Task DeleteGameAsync(int gameId)
    {
        await _repository.DeleteGameAsync(gameId);
    }

    public async Task AddSeasonAsync(Season season)
    {
        await _repository.AddSeasonAsync(season);
    }

    public async Task UpdateSeasonAsync(Season season)
    {
        await _repository.UpdateSeasonAsync(season);
    }

    public async Task DeleteSeasonAsync(int seasonId)
    {
        await _repository.DeleteSeasonAsync(seasonId);
    }

    public async Task AddPlayerAsync(Player player)
    {
        await _repository.AddPlayerAsync(player);
    }

    public async Task UpdatePlayerAsync(Player player)
    {
        await _repository.UpdatePlayerAsync(player);
    }

    public async Task DeletePlayerAsync(int playerId)
    {
        await _repository.DeletePlayerAsync(playerId);
    }

    public async Task<List<Player>> GetAllPlayersAsync()
    {
        return await _repository.GetAllPlayersAsync();
    }
}
