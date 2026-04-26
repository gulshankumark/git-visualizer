// src/GitVisualizer/Services/IGitSimulatorService.cs
using GitVisualizer.Models;

namespace GitVisualizer.Services;

public interface IGitSimulatorService
{
    bool IsProcessing { get; }
    IReadOnlyList<CommandHistoryEntry> CommandHistory { get; }
    event Action? StateChanged;

    /// <summary>Execute a raw git command string (e.g. "git init") and append to history.</summary>
    Task<CommandResult> ExecuteCommandAsync(string rawCommand);

    /// <summary>Clear terminal display history (non-destructive; git repo state preserved).</summary>
    Task ClearAsync();

    /// <summary>Destructive sandbox reset — clears history and resets repo state.</summary>
    Task ResetAsync();

    /// <summary>Initialize a new git repository.</summary>
    Task<CommandResult> InitRepoAsync();

    /// <summary>Stage a file path. Pass "." to stage all changed files.</summary>
    Task<CommandResult> AddAsync(string filepath = ".");

    /// <summary>Create a new commit with the given message.</summary>
    Task<CommandResult> CommitAsync(string message);

    /// <summary>Create a new branch with the given name.</summary>
    Task<CommandResult> CreateBranchAsync(string name);

    /// <summary>Checkout a branch; set createBranch=true to create+checkout (git checkout -b).</summary>
    Task<CommandResult> CheckoutAsync(string @ref, bool createBranch = false);

    /// <summary>Merge the given branch into the current branch.</summary>
    Task<CommandResult> MergeAsync(string branch);

    /// <summary>Return the commit log, newest first.</summary>
    Task<CommandResult> GetLogAsync(int depth = 20);
}
