using System;

namespace IsotopesStats.Domain.Models;

public record GameStatsExtendedView : StatEntry
{
    public new string PlayerName { get; set; } = string.Empty;

    public int SeasonId { get; set; }

    public string SeasonName { get; set; } = string.Empty;

    public int GameNumber { get; set; }

    public DateTime Date { get; set; }

    public string Diamond { get; set; } = string.Empty;

    public bool IsHome { get; set; }

    public int OpponentId { get; set; }

    public GameType GameType { get; set; }

    public bool GameIsDeleted { get; set; }

    public string OpponentName { get; set; } = string.Empty;

    public string OpponentShortName { get; set; } = string.Empty;

    public int? VisitingTeamScore { get; set; }

    public int? HomeTeamScore { get; set; }

    // Calculated properties from Isotopes perspective
    public int? OurScore => IsHome ? HomeTeamScore : VisitingTeamScore;
    public int? OpponentScore => IsHome ? VisitingTeamScore : HomeTeamScore;
}
