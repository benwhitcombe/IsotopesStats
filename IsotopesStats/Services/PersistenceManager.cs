namespace IsotopesStats.Services;

public class PersistenceManager : IDisposable
{
    private readonly LocalStorageService _localStorage;
    private readonly SharedSessionState _sharedState;
    private readonly PlayerStatsState _playerStatsState;
    private readonly GameStatsState _gameStatsState;

    private const string SharedStateKey = "isotopes_shared_state";
    private const string PlayerStatsKey = "isotopes_player_stats_state";
    private const string GameStatsKey = "isotopes_game_stats_state";

    public PersistenceManager(
        LocalStorageService localStorage,
        SharedSessionState sharedState,
        PlayerStatsState playerStatsState,
        GameStatsState gameStatsState)
    {
        _localStorage = localStorage;
        _sharedState = sharedState;
        _playerStatsState = playerStatsState;
        _gameStatsState = gameStatsState;

        _sharedState.OnChanged += SaveSharedState;
        _playerStatsState.OnChanged += SavePlayerStatsState;
        _gameStatsState.OnChanged += SaveGameStatsState;
    }

    public async Task InitializeAsync()
    {
        SharedSessionState? shared = await _localStorage.GetItemAsync<SharedSessionState>(SharedStateKey);
        if (shared != null) _sharedState.LoadFrom(shared);

        PlayerStatsState? player = await _localStorage.GetItemAsync<PlayerStatsState>(PlayerStatsKey);
        if (player != null) _playerStatsState.LoadFrom(player);

        GameStatsState? game = await _localStorage.GetItemAsync<GameStatsState>(GameStatsKey);
        if (game != null) _gameStatsState.LoadFrom(game);
    }

    private async void SaveSharedState() => await _localStorage.SetItemAsync(SharedStateKey, _sharedState);
    private async void SavePlayerStatsState() => await _localStorage.SetItemAsync(PlayerStatsKey, _playerStatsState);
    private async void SaveGameStatsState() => await _localStorage.SetItemAsync(GameStatsKey, _gameStatsState);

    public void Dispose()
    {
        _sharedState.OnChanged -= SaveSharedState;
        _playerStatsState.OnChanged -= SavePlayerStatsState;
        _gameStatsState.OnChanged -= SaveGameStatsState;
    }
}
