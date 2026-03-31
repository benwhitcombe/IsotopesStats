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
}
