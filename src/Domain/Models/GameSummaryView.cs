namespace IsotopesStats.Domain.Models;

public record GameSummaryView
{
    public int GameId { get; set; }

    public int SeasonId { get; set; }

    public int GameNumber { get; set; }

    public DateTime Date { get; set; }

    public string Diamond { get; set; } = string.Empty;

    public bool IsHome { get; set; }

    public int OpponentId { get; set; }

    public GameType GameType { get; set; }

    public bool GameIsDeleted { get; set; }

    public string OpponentName { get; set; } = string.Empty;

    public string OpponentShortName { get; set; } = string.Empty;

    public int PlayerCount { get; set; }

    public int TeamRuns { get; set; }

    public int TeamHits { get; set; }

    public int TeamHRs { get; set; }

    public int TeamBBs { get; set; }

    public int TeamAB { get; set; }

    public int TeamPA { get; set; }

    public int TeamRBI { get; set; }

    public int? VisitingTeamScore { get; set; }

    public int? HomeTeamScore { get; set; }
}
