namespace IsotopesStats.Website.Services;

public class PersistenceManager : IDisposable
{
    private readonly SessionStorageService _sessionStorage;
    private readonly SharedSessionState _sharedState;
    private readonly PlayerStatsState _playerStatsState;
    private readonly GameStatsState _gameStatsState;

    private const string SharedStateKey = "isotopes_shared_state";
    private const string PlayerStatsKey = "isotopes_player_stats_state";
    private const string GameStatsKey = "isotopes_game_stats_state";

    public PersistenceManager(
        SessionStorageService sessionStorage,
        SharedSessionState sharedState,
        PlayerStatsState playerStatsState,
        GameStatsState gameStatsState)
    {
        _sessionStorage = sessionStorage;
        _sharedState = sharedState;
        _playerStatsState = playerStatsState;
        _gameStatsState = gameStatsState;

        _sharedState.OnChanged += SaveSharedState;
        _playerStatsState.OnChanged += SavePlayerStatsState;
        _gameStatsState.OnChanged += SaveGameStatsState;
    }

    public async Task InitializeAsync()
    {
        SharedSessionState? shared = await _sessionStorage.GetItemAsync<SharedSessionState>(SharedStateKey);
        if (shared != null) _sharedState.LoadFrom(shared);

        PlayerStatsState? player = await _sessionStorage.GetItemAsync<PlayerStatsState>(PlayerStatsKey);
        if (player != null) _playerStatsState.LoadFrom(player);

        GameStatsState? game = await _sessionStorage.GetItemAsync<GameStatsState>(GameStatsKey);
        if (game != null) _gameStatsState.LoadFrom(game);
    }

    private async void SaveSharedState() => await _sessionStorage.SetItemAsync(SharedStateKey, _sharedState);
    private async void SavePlayerStatsState() => await _sessionStorage.SetItemAsync(PlayerStatsKey, _playerStatsState);
    private async void SaveGameStatsState() => await _sessionStorage.SetItemAsync(GameStatsKey, _gameStatsState);

    public void Dispose()
    {
        _sharedState.OnChanged -= SaveSharedState;
        _playerStatsState.OnChanged -= SavePlayerStatsState;
        _gameStatsState.OnChanged -= SaveGameStatsState;
    }
}
