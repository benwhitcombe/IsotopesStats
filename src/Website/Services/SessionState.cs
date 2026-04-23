using System.Text.Json.Serialization;

namespace IsotopesStats.Website.Services;

public record SharedSessionState
{
    public const int AllSeasonsId = -1;
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
