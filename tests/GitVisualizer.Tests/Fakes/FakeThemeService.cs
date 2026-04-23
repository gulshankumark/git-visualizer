// tests/GitVisualizer.Tests/Fakes/FakeThemeService.cs
using GitVisualizer.Services;
using MudBlazor;

namespace GitVisualizer.Tests.Fakes;

internal sealed class FakeThemeService : IThemeService
{
    public bool     IsDarkMode { get; set; } = true;
    public MudTheme Theme      => IsDarkMode ? AppTheme.Dark : AppTheme.Light;
    public event Action? ThemeChanged;
    public Task InitializeAsync() => Task.CompletedTask;
    public Task ToggleAsync()
    {
        IsDarkMode = !IsDarkMode;
        ThemeChanged?.Invoke();
        return Task.CompletedTask;
    }
}
