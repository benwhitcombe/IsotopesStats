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

    public async Task<Game?> GetGameAsync(int gameId)
    {
        return await _repository.GetGameAsync(gameId);
    }

    public async Task<List<StatEntry>> GetGameStatsAsync(int gameId)
    {
        return await _repository.GetGameStatsAsync(gameId);
    }

    public async Task UpdateGameWithStatsAsync(Game game, List<StatEntry> stats)
    {
        await _repository.UpdateGameWithStatsAsync(game, stats);
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

    public async Task AddPlayerToSeasonAsync(int playerId, int seasonId)
    {
        await _repository.AddPlayerToSeasonAsync(playerId, seasonId);
    }

    public async Task<List<Season>> GetSeasonsForPlayerAsync(int playerId)
    {
        return await _repository.GetSeasonsForPlayerAsync(playerId);
    }

    public async Task AddPlayerAsync(Player player, int seasonId)
    {
        await _repository.AddPlayerAsync(player, seasonId);
    }

    public async Task UpdatePlayerAsync(Player player)
    {
        await _repository.UpdatePlayerAsync(player);
    }

    public async Task DeletePlayerAsync(int playerId)
    {
        await _repository.DeletePlayerAsync(playerId);
    }

    public async Task DeletePlayerFromSeasonAsync(int playerId, int seasonId)
    {
        await _repository.DeletePlayerFromSeasonAsync(playerId, seasonId);
    }

    public async Task<List<Player>> GetAllPlayersAsync()
    {
        return await _repository.GetAllPlayersAsync();
    }

    public async Task<List<Opponent>> GetOpponentsAsync(int seasonId)
    {
        return await _repository.GetOpponentsAsync(seasonId);
    }

    public async Task<List<Opponent>> GetAllOpponentsAsync()
    {
        return await _repository.GetAllOpponentsAsync();
    }

    public async Task AddOpponentAsync(Opponent opponent, int seasonId)
    {
        await _repository.AddOpponentAsync(opponent, seasonId);
    }

    public async Task UpdateOpponentAsync(Opponent opponent)
    {
        await _repository.UpdateOpponentAsync(opponent);
    }

    public async Task DeleteOpponentAsync(int opponentId)
    {
        await _repository.DeleteOpponentAsync(opponentId);
    }

    public async Task AddOpponentToSeasonAsync(int opponentId, int seasonId)
    {
        await _repository.AddOpponentToSeasonAsync(opponentId, seasonId);
    }

    public async Task DeleteOpponentFromSeasonAsync(int opponentId, int seasonId)
    {
        await _repository.DeleteOpponentFromSeasonAsync(opponentId, seasonId);
    }

    public async Task<List<Season>> GetSeasonsForOpponentAsync(int opponentId)
    {
        return await _repository.GetSeasonsForOpponentAsync(opponentId);
    }
}
