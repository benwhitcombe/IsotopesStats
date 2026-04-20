using Microsoft.JSInterop;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace IsotopesStats.Services;

public class SessionStorageService
{
    private readonly IJSRuntime _jsRuntime;
    private readonly JsonSerializerSettings _settings;

    public SessionStorageService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
        _settings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver(),
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
    }

    public async Task SetItemAsync<T>(string key, T value)
    {
        try
        {
            string json = JsonConvert.SerializeObject(value, _settings);
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

            return JsonConvert.DeserializeObject<T>(json, _settings);
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
