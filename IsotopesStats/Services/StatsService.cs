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

    public List<PlayerStatsSummary> GetStatsSummary()
    {
        return _repository.GetStatsSummary();
    }

    public PlayerStatsSummary GetTeamTotals()
    {
        return _repository.GetTeamTotals();
    }

    public List<Player> GetPlayers()
    {
        return _repository.GetPlayers();
    }

    public List<StatEntry> GetAllGameStats()
    {
        return _repository.GetAllGameStats();
    }

    public void AddGameWithStats(Game game, List<StatEntry> stats)
    {
        _repository.AddGameWithStats(game, stats);
    }
}
