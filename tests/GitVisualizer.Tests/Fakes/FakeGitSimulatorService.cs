// tests/GitVisualizer.Tests/Fakes/FakeGitSimulatorService.cs
using GitVisualizer.Models;
using GitVisualizer.Services;

namespace GitVisualizer.Tests.Fakes;

internal sealed class FakeGitSimulatorService : IGitSimulatorService
{
    private readonly List<CommandHistoryEntry> _history = new();

    public bool IsProcessing { get; set; }
    public IReadOnlyList<CommandHistoryEntry> CommandHistory => _history;
    public RepoState? CurrentState { get; set; }
    public bool SessionRestored { get; set; }
    public bool SchemaMismatch { get; set; }
    public event Action? StateChanged;

    public List<string> ExecutedCommands { get; } = new();
    public int ClearCount  { get; private set; }
    public int ResetCount  { get; private set; }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task SetSchemaMismatchFlagAsync()
    {
        SchemaMismatch = true;
        return Task.CompletedTask;
    }

    public Task<CommandResult> ExecuteCommandAsync(string rawCommand)
    {
        ExecutedCommands.Add(rawCommand);
        var result = new CommandResult(true, $"Executed: {rawCommand}", null, null, null);
        _history.Add(new CommandHistoryEntry(rawCommand, result, DateTime.UtcNow));
        StateChanged?.Invoke();
        return Task.FromResult(result);
    }

    public Task ClearAsync()
    {
        ClearCount++;
        _history.Clear();
        StateChanged?.Invoke();
        return Task.CompletedTask;
    }

    public Task ResetAsync()
    {
        ResetCount++;
        _history.Clear();
        StateChanged?.Invoke();
        return Task.CompletedTask;
    }

    public Task<CommandResult> InitRepoAsync() =>
        Task.FromResult(new CommandResult(true, "Initialized.", null, null,
            new RepoState(true, "main", [], [])));

    public Task<CommandResult> AddAsync(string filepath = ".") =>
        Task.FromResult(new CommandResult(true, "Staged.", null, null, null));

    public Task<CommandResult> CommitAsync(string message) =>
        Task.FromResult(new CommandResult(true, $"Committed: {message}", null, null, null));

    public Task<CommandResult> CreateBranchAsync(string name) =>
        Task.FromResult(new CommandResult(true, $"Branch '{name}' created.", null, null, null));

    public Task<CommandResult> CheckoutAsync(string @ref, bool createBranch = false) =>
        Task.FromResult(new CommandResult(true, $"Switched to '{@ref}'.", null, null, null));

    public Task<CommandResult> MergeAsync(string branch) =>
        Task.FromResult(new CommandResult(true, $"Merged '{branch}'.", null, null, null));

    public Task<CommandResult> GetLogAsync(int depth = 20) =>
        Task.FromResult(new CommandResult(true, "(no commits)", null, null, null));

    public void AddHistoryEntry(string command, CommandResult result)
        => _history.Add(new CommandHistoryEntry(command, result, DateTime.UtcNow));

    public void RaiseStateChanged() => StateChanged?.Invoke();
}
