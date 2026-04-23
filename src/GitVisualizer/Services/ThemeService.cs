// src/GitVisualizer/Services/ThemeService.cs
using Microsoft.JSInterop;
using MudBlazor;

namespace GitVisualizer.Services;

public sealed class ThemeService : IThemeService, IAsyncDisposable
{
    private const string StorageKey = "gitvis.prefs.theme";

    private readonly ILocalStorageService _localStorage;
    private readonly IJSRuntime _js;
    private IJSObjectReference? _module;

    public bool IsDarkMode { get; private set; } = true;
    public MudTheme Theme => IsDarkMode ? AppTheme.Dark : AppTheme.Light;

    public event Action? ThemeChanged;

    public ThemeService(ILocalStorageService localStorage, IJSRuntime js)
    {
        _localStorage = localStorage;
        _js = js;
    }

    public async Task InitializeAsync()
    {
        var stored = _localStorage.GetItem<string>(StorageKey);
        IsDarkMode = stored != "light";
        await SetDocumentThemeAsync();
        ThemeChanged?.Invoke();
    }

    public async Task ToggleAsync()
    {
        IsDarkMode = !IsDarkMode;
        _localStorage.SetItem(StorageKey, IsDarkMode ? "dark" : "light");
        await SetDocumentThemeAsync();
        ThemeChanged?.Invoke();
    }

    private async Task SetDocumentThemeAsync()
    {
        try
        {
            var module = await GetModuleAsync();
            await module.InvokeVoidAsync("setDocumentTheme", IsDarkMode ? "dark" : "light");
        }
        catch (JSException)
        {
            // Silently swallow JS exceptions during WASM cold start
        }
    }

    private async ValueTask<IJSObjectReference> GetModuleAsync()
        => _module ??= await _js.InvokeAsync<IJSObjectReference>(
               "import", "./js/split-pane-interop.js");

    public async ValueTask DisposeAsync()
    {
        if (_module is not null)
        {
            await _module.DisposeAsync();
            _module = null;
        }
    }
}