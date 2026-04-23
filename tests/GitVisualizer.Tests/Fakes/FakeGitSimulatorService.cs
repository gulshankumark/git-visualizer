// tests/GitVisualizer.Tests/Fakes/FakeGitSimulatorService.cs
using GitVisualizer.Models;
using GitVisualizer.Services;

namespace GitVisualizer.Tests.Fakes;

internal sealed class FakeGitSimulatorService : IGitSimulatorService
{
    private readonly List<CommandHistoryEntry> _history = new();

    public bool IsProcessing { get; set; }
    public IReadOnlyList<CommandHistoryEntry> CommandHistory => _history;
    public event Action? StateChanged;

    public List<string> ExecutedCommands { get; } = new();
    public int ClearCount  { get; private set; }
    public int ResetCount  { get; private set; }

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

    public void AddHistoryEntry(string command, CommandResult result)
        => _history.Add(new CommandHistoryEntry(command, result, DateTime.UtcNow));
}
