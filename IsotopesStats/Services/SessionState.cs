namespace IsotopesStats.Services;

public class SharedSessionState
{
    public int SelectedSeasonId { get; set; } = 0;
}

public class PlayerStatsState
{
    public string ActiveView { get; set; } = "Standard";
    
    // Standard View State
    public string StandardSortColumn { get; set; } = "AVG";
    public bool StandardIsAscending { get; set; } = false;

    // Hits View State
    public string HitsSortColumn { get; set; } = "SLG";
    public bool HitsIsAscending { get; set; } = false;

    // Outcomes View State
    public string OutcomesSortColumn { get; set; } = "OBP";
    public bool OutcomesIsAscending { get; set; } = false;

    public string CurrentSortColumn => ActiveView switch {
        "Standard" => StandardSortColumn,
        "Hits" => HitsSortColumn,
        "Outcomes" => OutcomesSortColumn,
        _ => "AVG"
    };

    public bool IsAscending => ActiveView switch {
        "Standard" => StandardIsAscending,
        "Hits" => HitsIsAscending,
        "Outcomes" => OutcomesIsAscending,
        _ => false
    };
}

public class GameStatsState
{
    public HashSet<int> ExpandedGames { get; set; } = new();
    public string FilterText { get; set; } = string.Empty;
}
