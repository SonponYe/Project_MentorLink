using System.Text.Json;
using MentorLink.Shared.Dtos;
using Microsoft.JSInterop;

namespace MentorLink.Client.Services;

/// <summary>Client-side session state, persisted to localStorage so refreshes keep you signed in.</summary>
public class AppState
{
    private const string StorageKey = "mentorlink-user";
    private readonly IJSRuntime _js;
    private bool _loaded;

    public AppState(IJSRuntime js) => _js = js;

    public UserDto? CurrentUser { get; private set; }
    public event Action? OnChange;

    public async Task EnsureLoadedAsync()
    {
        if (_loaded) return;
        _loaded = true;
        try
        {
            var json = await _js.InvokeAsync<string?>("mlStore.get", StorageKey);
            if (!string.IsNullOrEmpty(json))
                CurrentUser = JsonSerializer.Deserialize<UserDto>(json);
        }
        catch { /* localStorage unavailable — stay signed out */ }
    }

    public async Task SignInAsync(UserDto user)
    {
        CurrentUser = user;
        await _js.InvokeVoidAsync("mlStore.set", StorageKey, JsonSerializer.Serialize(user));
        OnChange?.Invoke();
    }

    public async Task SignOutAsync()
    {
        CurrentUser = null;
        await _js.InvokeVoidAsync("mlStore.del", StorageKey);
        OnChange?.Invoke();
    }
}
