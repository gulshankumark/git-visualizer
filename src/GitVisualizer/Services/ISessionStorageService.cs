// src/GitVisualizer/Services/ISessionStorageService.cs
using GitVisualizer.Models;

namespace GitVisualizer.Services;

public interface ISessionStorageService : IAsyncDisposable
{
    bool SchemaMismatchDetected { get; }
    
    Task<RepoState?> LoadStateAsync();
    Task SaveStateAsync(RepoState state);
    
    Task<UserPrefs?> LoadPrefsAsync();
    Task SavePrefsAsync(UserPrefs prefs);
}

public record UserPrefs(string Theme);
