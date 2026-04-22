using Microsoft.JSInterop;
using System.Text.Json;

namespace IsotopesStats.Services;

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
            string json = JsonSerializer.Serialize(value, _options);
            await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", key, json);
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
            string json = await _jsRuntime.InvokeAsync<string>("sessionStorage.getItem", key);
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
        await _jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", key);
    }
}
