namespace IsotopesStats.Domain.Models;

public record SeasonPlayers
{
    public int SeasonId { get; set; }

    public int PlayerId { get; set; }
}
