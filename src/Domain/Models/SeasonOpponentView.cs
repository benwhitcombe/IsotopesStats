namespace IsotopesStats.Domain.Models;

public record SeasonOpponentView
{
    public int SeasonId { get; set; }

    public int OpponentId { get; set; }

    public string OpponentName { get; set; } = string.Empty;

    public string OpponentShortName { get; set; } = string.Empty;
}
