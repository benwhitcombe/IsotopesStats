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

    public List<Season> GetSeasons()
    {
        return _repository.GetSeasons();
    }

    public List<PlayerStatsSummary> GetStatsSummary(int seasonId)
    {
        return _repository.GetStatsSummary(seasonId);
    }

    public PlayerStatsSummary GetTeamTotals(int seasonId)
    {
        return _repository.GetTeamTotals(seasonId);
    }

    public List<Player> GetPlayers(int seasonId)
    {
        return _repository.GetPlayers(seasonId);
    }

    public List<StatEntry> GetAllGameStats(int seasonId)
    {
        return _repository.GetAllGameStats(seasonId);
    }

    public void AddGameWithStats(Game game, List<StatEntry> stats)
    {
        _repository.AddGameWithStats(game, stats);
    }
}
