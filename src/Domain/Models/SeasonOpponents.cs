namespace IsotopesStats.Domain.Models;

public record SeasonOpponents
{
    public int SeasonId { get; set; }

    public int OpponentId { get; set; }

    public string? Name { get; set; }

    public string? ShortName { get; set; }
}
