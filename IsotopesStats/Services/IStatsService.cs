using IsotopesStats.Models;

namespace IsotopesStats.Services;

public interface IStatsService
{
    // Seasons
    Task<List<Season>> GetSeasonsAsync();
    Task<int> AddSeasonAsync(Season season);
    Task UpdateSeasonAsync(Season season);
    Task DeleteSeasonAsync(int seasonId);
    Task<bool> IsSeasonNameUniqueAsync(string name, int excludeId = 0);

    // Players
    Task<List<Player>> GetAllPlayersAsync();
    Task<List<Player>> GetPlayersAsync(int seasonId);
    Task<int> AddPlayerAsync(Player player, int seasonId);
    Task UpdatePlayerAsync(Player player);
    Task DeletePlayerAsync(int playerId);
    Task AddPlayerToSeasonAsync(int playerId, int seasonId);
    Task DeletePlayerFromSeasonAsync(int playerId, int seasonId);
    Task<bool> IsPlayerNameUniqueAsync(string name, int excludeId = 0);
    Task<List<Season>> GetSeasonsForPlayerAsync(int playerId);

    // Opponents
    Task<List<Opponent>> GetAllOpponentsAsync();
    Task<List<Opponent>> GetOpponentsAsync(int seasonId);
    Task<int> AddOpponentAsync(Opponent opponent, int seasonId);
    Task UpdateOpponentAsync(Opponent opponent);
    Task DeleteOpponentAsync(int opponentId);
    Task<bool> IsOpponentNameUniqueAsync(string name, int excludeId = 0);
    Task<List<SeasonOpponents>> GetSeasonsForOpponentAsync(int opponentId);
    Task AddOpponentToSeasonAsync(int opponentId, int seasonId, string name, string? shortName = null);
    Task DeleteOpponentFromSeasonAsync(int opponentId, int seasonId);

    // Games & Stats
    Task<Game?> GetGameAsync(int gameId);
    Task<List<Game>> GetGamesBySeasonAsync(int seasonId);
    Task<List<StatEntry>> GetGameStatsAsync(int gameId);
    Task<List<PlayerStatsSummary>> GetStatsSummaryAsync(int seasonId);
    Task<PlayerStatsSummary> GetTeamTotalsAsync(int seasonId);
    Task<int> GetMostRecentStatsSeasonIdAsync();
    Task AddGameWithStatsAsync(Game game, List<StatEntry> stats);
    Task UpdateGameWithStatsAsync(Game game, List<StatEntry> stats);
    Task DeleteGameAsync(int gameId);
    Task<int> GetNextGameNumberAsync(int seasonId);
    Task<List<GameStatsExtendedView>> GetAllGameStatsAsync(int seasonId);
    Task<List<GameStatsExtendedView>> GetPlayerGameLogAsync(string playerName, int seasonId);
    Task<List<GameSummaryView>> GetGameSummariesAsync(int seasonId);
    Task<List<GameStatsExtendedView>> GetExtendedGameStatsAsync(int gameId);
}
