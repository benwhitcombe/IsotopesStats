using System.Text.Json.Serialization;

namespace IsotopesStats.Services;

public class SharedSessionState
{
    public int SelectedSeasonId { get; set; } = 0;
    public string ReturnUrl { get; set; } = "players"; // Default to players
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

public class GameUIState
{
    [JsonInclude]
    public int GameId { get; set; }
    [JsonInclude]
    public bool IsExpanded { get; set; }
    [JsonInclude]
    public string SortColumn { get; set; } = "BO";
    [JsonInclude]
    public bool IsAscending { get; set; } = true;
}

public class GameStatsState
{
    [JsonInclude]
    public string FilterText { get; set; } = string.Empty;
    
    [JsonInclude]
    public Dictionary<int, GameUIState> GameUIStates { get; set; } = new();

    public GameUIState GetState(int gameId)
    {
        if (!GameUIStates.TryGetValue(gameId, out GameUIState? state))
        {
            state = new GameUIState { GameId = gameId };
            GameUIStates[gameId] = state;
        }
        return state!;
    }
}