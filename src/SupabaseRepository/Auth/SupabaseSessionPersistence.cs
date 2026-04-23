using Supabase.Gotrue;
using Supabase.Gotrue.Interfaces;
using Microsoft.JSInterop;
using Newtonsoft.Json;

namespace IsotopesStats.SupabaseRepository.Auth;

internal class SupabaseSessionPersistence : IGotrueSessionPersistence<Session>
{
    private readonly IJSRuntime _jsRuntime;
    private const string SessionKey = "supabase_session";

    public SupabaseSessionPersistence(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public void SaveSession(Session session)
    {
        if (session == null || string.IsNullOrEmpty(session.AccessToken)) return;
        
        try
        {
            string json = JsonConvert.SerializeObject(session);
            if (_jsRuntime is IJSInProcessRuntime inProcess)
            {
                inProcess.InvokeVoid("sessionStorage.setItem", SessionKey, json);
            }
            else
            {
                _ = _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", SessionKey, json);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving supabase session: {ex.Message}");
        }
    }

    public void DestroySession()
    {
        try
        {
            if (_jsRuntime is IJSInProcessRuntime inProcess)
            {
                inProcess.InvokeVoid("sessionStorage.removeItem", SessionKey);
            }
            else
            {
                _ = _jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", SessionKey);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error destroying supabase session: {ex.Message}");
        }
    }

    public Session? LoadSession()
    {
        try
        {
            if (_jsRuntime is IJSInProcessRuntime inProcess)
            {
                string? json = inProcess.Invoke<string>("sessionStorage.getItem", SessionKey);
                if (string.IsNullOrEmpty(json)) return null;

                return JsonConvert.DeserializeObject<Session>(json);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading supabase session: {ex.Message}");
        }
        return null;
    }
}

