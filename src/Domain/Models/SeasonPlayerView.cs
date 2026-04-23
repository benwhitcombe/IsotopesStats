namespace IsotopesStats.Domain.Models;

public record SeasonPlayerView
{
    public int SeasonId { get; set; }

    public int PlayerId { get; set; }

    public string PlayerName { get; set; } = string.Empty;
}
