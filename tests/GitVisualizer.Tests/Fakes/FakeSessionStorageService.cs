// tests/GitVisualizer.Tests/Fakes/FakeSessionStorageService.cs
using GitVisualizer.Services;
using GitVisualizer.Models;

namespace GitVisualizer.Tests.Fakes;

internal sealed class FakeSessionStorageService : ISessionStorageService
{
    public bool SchemaMismatchDetected { get; set; }
    
    private RepoState? _savedState;
    private UserPrefs? _savedPrefs;

    public Task<RepoState?> LoadStateAsync()
    {
        return Task.FromResult(_savedState);
    }

    public Task SaveStateAsync(RepoState state)
    {
        _savedState = state;
        return Task.CompletedTask;
    }

    public Task<UserPrefs?> LoadPrefsAsync()
    {
        return Task.FromResult(_savedPrefs);
    }

    public Task SavePrefsAsync(UserPrefs prefs)
    {
        _savedPrefs = prefs;
        return Task.CompletedTask;
    }

    public Task ClearAllAsync()
    {
        _savedState = null;
        _savedPrefs = null;
        return Task.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}
