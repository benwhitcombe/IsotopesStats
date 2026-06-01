using Microsoft.JSInterop;
using System.Text.Json;

namespace IsotopesStats.Website.Services;

public class SessionStorageService
{
    private readonly IJSRuntime _jsRuntime;
    private readonly JsonSerializerOptions _options;

    public SessionStorageService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
        _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task SetItemAsync<T>(string key, T value)
    {
        try
        {
            if (!await IsAvailable()) return;
            string json = JsonSerializer.Serialize(value, _options);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving to session storage: {ex.Message}");
        }
    }

    public async Task<T?> GetItemAsync<T>(string key)
    {
        try
        {
            if (!await IsAvailable()) return default;
            string json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", key);
            if (string.IsNullOrEmpty(json))
                return default;

            return JsonSerializer.Deserialize<T>(json, _options);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading from session storage: {ex.Message}");
            return default;
        }
    }

    public async Task RemoveItemAsync(string key)
    {
        try
        {
            if (!await IsAvailable()) return;
            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error removing from session storage: {ex.Message}");
        }
    }

    private bool? _isAvailable;
    private async Task<bool> IsAvailable()
    {
        if (_isAvailable.HasValue) return _isAvailable.Value;
        try
        {
            _isAvailable = await _jsRuntime.InvokeAsync<bool>("eval", "typeof localStorage !== 'undefined' && (()=>{try{localStorage.setItem('test','1');localStorage.removeItem('test');return true;}catch(e){return false;}})()");
        }
        catch
        {
            _isAvailable = false;
        }
        return _isAvailable.Value;
    }
}
