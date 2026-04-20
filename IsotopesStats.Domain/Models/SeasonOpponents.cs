namespace IsotopesStats.Models;

public record SeasonOpponents
{
    public int SeasonId { get; set; }

    public int OpponentId { get; set; }

    public string? Name { get; set; }
}
