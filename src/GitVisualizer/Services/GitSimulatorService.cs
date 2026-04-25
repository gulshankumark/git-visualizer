// src/GitVisualizer/Services/GitSimulatorService.cs
using GitVisualizer.Models;

namespace GitVisualizer.Services;

/// <summary>
/// Stub implementation of IGitSimulatorService.
/// Handles a small set of git commands locally; full JS-backed implementation in Story 2.3.
/// </summary>
public sealed class GitSimulatorService : IGitSimulatorService
{
    private readonly List<CommandHistoryEntry> _history = new();
    private bool _isProcessing;

    public bool IsProcessing => _isProcessing;
    public IReadOnlyList<CommandHistoryEntry> CommandHistory => _history;
    public event Action? StateChanged;

    public async Task<CommandResult> ExecuteCommandAsync(string rawCommand)
    {
        _isProcessing = true;
        StateChanged?.Invoke();

        try
        {
            await Task.Delay(300);

            var result = RunCommand(rawCommand.Trim());
            _history.Add(new CommandHistoryEntry(rawCommand, result, DateTime.UtcNow));
            return result;
        }
        finally
        {
            _isProcessing = false;
            StateChanged?.Invoke();
        }
    }

    private static CommandResult RunCommand(string command) => command switch
    {
        "git init"   => new CommandResult(true,  "Initialized empty Git repository in .git/", null, null, null),
        "git status" => new CommandResult(true,  "On branch main\nNothing to commit, working tree clean", null, null, null),
        _            => new CommandResult(false, "", $"'{command}' is not yet supported. Full git support arrives in Story 2.3.", null, null)
    };

    public Task ClearAsync()
    {
        _history.Clear();
        StateChanged?.Invoke();
        return Task.CompletedTask;
    }

    public Task ResetAsync()
    {
        _history.Clear();
        StateChanged?.Invoke();
        return Task.CompletedTask;
    }
}
