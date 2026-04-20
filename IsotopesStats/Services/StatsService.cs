using IsotopesStats.Models;
using IsotopesStats.Domain.Interfaces;

namespace IsotopesStats.Services;

public class StatsService : IStatsService
{
    private readonly IStatsRepository _repository;

    public StatsService(IStatsRepository repository)
    {
        _repository = repository;
    }

    // --- SEASONS ---
    public Task<List<Season>> GetSeasonsAsync() => _repository.GetSeasonsAsync();
    public Task<int> AddSeasonAsync(Season season) => _repository.AddSeasonAsync(season);
    public Task UpdateSeasonAsync(Season season) => _repository.UpdateSeasonAsync(season);
    public Task DeleteSeasonAsync(int seasonId) => _repository.DeleteSeasonAsync(seasonId);
    public Task<bool> IsSeasonNameUniqueAsync(string name, int excludeId = 0) => _repository.IsSeasonNameUniqueAsync(name, excludeId);

    // --- PLAYERS ---
    public Task<List<Player>> GetAllPlayersAsync() => _repository.GetAllPlayersAsync();
    public Task<List<Player>> GetPlayersAsync(int seasonId) => _repository.GetPlayersAsync(seasonId);
    public Task<int> AddPlayerAsync(Player player, int seasonId) => _repository.AddPlayerAsync(player, seasonId);
    public Task UpdatePlayerAsync(Player player) => _repository.UpdatePlayerAsync(player);
    public Task DeletePlayerAsync(int playerId) => _repository.DeletePlayerAsync(playerId);
    public Task AddPlayerToSeasonAsync(int playerId, int seasonId) => _repository.AddPlayerToSeasonAsync(playerId, seasonId);
    public Task DeletePlayerFromSeasonAsync(int playerId, int seasonId) => _repository.DeletePlayerFromSeasonAsync(playerId, seasonId);
    public Task<bool> IsPlayerNameUniqueAsync(string name, int excludeId = 0) => _repository.IsPlayerNameUniqueAsync(name, excludeId);
    public Task<List<Season>> GetSeasonsForPlayerAsync(int playerId) => _repository.GetSeasonsForPlayerAsync(playerId);

    // --- OPPONENTS ---
    public Task<List<Opponent>> GetAllOpponentsAsync() => _repository.GetAllOpponentsAsync();
    public Task<List<Opponent>> GetOpponentsAsync(int seasonId) => _repository.GetOpponentsAsync(seasonId);
    public Task<int> AddOpponentAsync(Opponent opponent, int seasonId) => _repository.AddOpponentAsync(opponent, seasonId);
    public Task UpdateOpponentAsync(Opponent opponent) => _repository.UpdateOpponentAsync(opponent);
    public Task DeleteOpponentAsync(int opponentId) => _repository.DeleteOpponentAsync(opponentId);
    public Task<bool> IsOpponentNameUniqueAsync(string name, int excludeId = 0) => _repository.IsOpponentNameUniqueAsync(name, excludeId);
    public Task<List<SeasonOpponents>> GetSeasonsForOpponentAsync(int opponentId) => _repository.GetSeasonsForOpponentAsync(opponentId);
    public Task AddOpponentToSeasonAsync(int opponentId, int seasonId, string? name = null) => _repository.AddOpponentToSeasonAsync(opponentId, seasonId, name);
    public Task DeleteOpponentFromSeasonAsync(int opponentId, int seasonId) => _repository.DeleteOpponentFromSeasonAsync(opponentId, seasonId);

    // --- GAMES & STATS ---
    public Task<Game?> GetGameAsync(int gameId) => _repository.GetGameAsync(gameId);
    public Task<List<Game>> GetGamesBySeasonAsync(int seasonId) => _repository.GetGamesBySeasonAsync(seasonId);
    public Task<List<StatEntry>> GetGameStatsAsync(int gameId) => _repository.GetGameStatsAsync(gameId);
    public Task<List<PlayerStatsSummary>> GetStatsSummaryAsync(int seasonId) => _repository.GetStatsSummaryAsync(seasonId);
    public Task<PlayerStatsSummary> GetTeamTotalsAsync(int seasonId) => _repository.GetTeamTotalsAsync(seasonId);
    public Task<int> GetMostRecentStatsSeasonIdAsync() => _repository.GetMostRecentStatsSeasonIdAsync();
    public Task AddGameWithStatsAsync(Game game, List<StatEntry> stats) => _repository.AddGameWithStatsAsync(game, stats);
    public Task UpdateGameWithStatsAsync(Game game, List<StatEntry> stats) => _repository.UpdateGameWithStatsAsync(game, stats);
    public Task DeleteGameAsync(int gameId) => _repository.DeleteGameAsync(gameId);
    public Task<List<GameStatsExtendedView>> GetAllGameStatsAsync(int seasonId) => _repository.GetAllGameStatsAsync(seasonId);
    public Task<List<GameStatsExtendedView>> GetPlayerGameLogAsync(string playerName, int seasonId) => _repository.GetPlayerGameLogAsync(playerName, seasonId);
    public Task<int> GetNextGameNumberAsync(int seasonId) => _repository.GetNextGameNumberAsync(seasonId);
    public Task<List<GameSummaryView>> GetGameSummariesAsync(int seasonId) => _repository.GetGameSummariesAsync(seasonId);
    public Task<List<GameStatsExtendedView>> GetExtendedGameStatsAsync(int gameId) => _repository.GetExtendedGameStatsAsync(gameId);
}
