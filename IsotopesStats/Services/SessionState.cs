using System.Text.Json.Serialization;

namespace IsotopesStats.Services;

public record SharedSessionState
{
    private int _selectedSeasonId = 0;
    public int SelectedSeasonId 
    { 
        get => _selectedSeasonId; 
        set { if (_selectedSeasonId != value) { _selectedSeasonId = value; OnChanged?.Invoke(); } } 
    }

    private string _returnUrl = "players";
    public string ReturnUrl 
    { 
        get => _returnUrl; 
        set { if (_returnUrl != value) { _returnUrl = value; OnChanged?.Invoke(); } } 
    }

    private bool _openMenuOnLoad = false;
    public bool OpenMenuOnLoad 
    { 
        get => _openMenuOnLoad; 
        set { if (_openMenuOnLoad != value) { _openMenuOnLoad = value; OnChanged?.Invoke(); } } 
    }

    public event Action? OnChanged;

    public void LoadFrom(SharedSessionState other)
    {
        _selectedSeasonId = other.SelectedSeasonId;
        _returnUrl = other.ReturnUrl;
        _openMenuOnLoad = other.OpenMenuOnLoad;
    }
}

public record PlayerStatsState
{
    private string _filterText = string.Empty;
    public string FilterText 
    { 
        get => _filterText; 
        set { if (_filterText != value) { _filterText = value; OnChanged?.Invoke(); } } 
    }

    private string _currentSortColumn = "AVG";
    public string CurrentSortColumn 
    { 
        get => _currentSortColumn; 
        set { if (_currentSortColumn != value) { _currentSortColumn = value; OnChanged?.Invoke(); } } 
    }

    private bool _isAscending = false;
    public bool IsAscending 
    { 
        get => _isAscending; 
        set { if (_isAscending != value) { _isAscending = value; OnChanged?.Invoke(); } } 
    }

    public event Action? OnChanged;

    public void LoadFrom(PlayerStatsState other)
    {
        _filterText = other.FilterText;
        _currentSortColumn = other.CurrentSortColumn;
        _isAscending = other.IsAscending;
    }
}

public record PlayerStatsLegacyState
{
    private string _filterText = string.Empty;
    public string FilterText 
    { 
        get => _filterText; 
        set { if (_filterText != value) { _filterText = value; OnChanged?.Invoke(); } } 
    }

    private string _activeView = "Standard";
    public string ActiveView 
    { 
        get => _activeView; 
        set { if (_activeView != value) { _activeView = value; OnChanged?.Invoke(); } } 
    }
    
    private string _standardSortColumn = "AVG";
    public string StandardSortColumn 
    { 
        get => _standardSortColumn; 
        set { if (_standardSortColumn != value) { _standardSortColumn = value; OnChanged?.Invoke(); } } 
    }

    private bool _standardIsAscending = false;
    public bool StandardIsAscending 
    { 
        get => _standardIsAscending; 
        set { if (_standardIsAscending != value) { _standardIsAscending = value; OnChanged?.Invoke(); } } 
    }

    private string _hitsSortColumn = "SLG";
    public string HitsSortColumn 
    { 
        get => _hitsSortColumn; 
        set { if (_hitsSortColumn != value) { _hitsSortColumn = value; OnChanged?.Invoke(); } } 
    }

    private bool _hitsIsAscending = false;
    public bool HitsIsAscending 
    { 
        get => _hitsIsAscending; 
        set { if (_hitsIsAscending != value) { _hitsIsAscending = value; OnChanged?.Invoke(); } } 
    }

    private string _outcomesSortColumn = "OBP";
    public string OutcomesSortColumn 
    { 
        get => _outcomesSortColumn; 
        set { if (_outcomesSortColumn != value) { _outcomesSortColumn = value; OnChanged?.Invoke(); } } 
    }

    private bool _outcomesIsAscending = false;
    public bool OutcomesIsAscending 
    { 
        get => _outcomesIsAscending; 
        set { if (_outcomesIsAscending != value) { _outcomesIsAscending = value; OnChanged?.Invoke(); } } 
    }

    [JsonIgnore]
    public string CurrentSortColumn => ActiveView switch {
        "Standard" => StandardSortColumn,
        "Hits" => HitsSortColumn,
        "Outcomes" => OutcomesSortColumn,
        _ => "AVG"
    };

    [JsonIgnore]
    public bool IsAscending => ActiveView switch {
        "Standard" => StandardIsAscending,
        "Hits" => HitsIsAscending,
        "Outcomes" => OutcomesIsAscending,
        _ => false
    };

    public event Action? OnChanged;

    public void LoadFrom(PlayerStatsLegacyState other)
    {
        _filterText = other.FilterText;
        _activeView = other.ActiveView;
        _standardSortColumn = other.StandardSortColumn;
        _standardIsAscending = other.StandardIsAscending;
        _hitsSortColumn = other.HitsSortColumn;
        _hitsIsAscending = other.HitsIsAscending;
        _outcomesSortColumn = other.OutcomesSortColumn;
        _outcomesIsAscending = other.OutcomesIsAscending;
    }
}

public record GameUIState
{
    public int GameId { get; set; }
    public bool IsExpanded { get; set; }
    public string SortColumn { get; set; } = "BO";
    public bool IsAscending { get; set; } = true;
}

public record GameStatsState
{
    private string _filterText = string.Empty;
    public string FilterText 
    { 
        get => _filterText; 
        set { if (_filterText != value) { _filterText = value; OnChanged?.Invoke(); } } 
    }
    
    public Dictionary<int, GameUIState> GameUIStates { get; set; } = new();

    public GameUIState GetState(int gameId)
    {
        if (!GameUIStates.TryGetValue(gameId, out GameUIState? state))
        {
            state = new GameUIState { GameId = gameId };
            GameUIStates[gameId] = state;
            OnChanged?.Invoke();
        }
        return state!;
    }

    public void TriggerChange() => OnChanged?.Invoke();

    public event Action? OnChanged;

    public void LoadFrom(GameStatsState other)
    {
        _filterText = other.FilterText;
        GameUIStates = other.GameUIStates;
    }
}
