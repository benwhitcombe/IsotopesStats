namespace IsotopesStats.Models;

public record GameManagementView
{
    public int Id { get; set; }

    public int SeasonId { get; set; }

    public int GameNumber { get; set; }

    public DateTime Date { get; set; }

    public string Diamond { get; set; } = string.Empty;

    public bool IsHome { get; set; }

    public int OpponentId { get; set; }

    public GameType GameType { get; set; }

    public bool IsDeleted { get; set; }

    public string OpponentName { get; set; } = string.Empty;

    public string OpponentShortName { get; set; } = string.Empty;
}
