namespace IsotopesStats.Models;

public record GameStatsExtendedView : StatEntry
{
    public string ExtendedPlayerName { get; set; } = string.Empty;

    public int SeasonId { get; set; }

    public int GameNumber { get; set; }

    public DateTime Date { get; set; }

    public string Diamond { get; set; } = string.Empty;

    public bool IsHome { get; set; }

    public int OpponentId { get; set; }

    public GameType GameType { get; set; }

    public bool GameIsDeleted { get; set; }

    public string OpponentName { get; set; } = string.Empty;
}
