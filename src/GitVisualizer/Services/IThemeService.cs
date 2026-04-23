// src/GitVisualizer/Services/IThemeService.cs
using MudBlazor;

namespace GitVisualizer.Services;

public interface IThemeService
{
    bool IsDarkMode { get; }
    MudTheme Theme { get; }
    Task InitializeAsync();
    Task ToggleAsync();
    event Action? ThemeChanged;
}
