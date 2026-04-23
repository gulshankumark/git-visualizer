// src/GitVisualizer/Services/IGitSimulatorService.cs
using GitVisualizer.Models;

namespace GitVisualizer.Services;

public interface IGitSimulatorService
{
    bool IsProcessing { get; }
    IReadOnlyList<CommandHistoryEntry> CommandHistory { get; }
    event Action? StateChanged;

    /// <summary>Execute a raw git command string and append to history.</summary>
    Task<CommandResult> ExecuteCommandAsync(string rawCommand);

    /// <summary>Clear terminal display history (non-destructive; git repo state preserved).</summary>
    Task ClearAsync();

    /// <summary>Destructive sandbox reset — clears git state and history.</summary>
    Task ResetAsync();
}
