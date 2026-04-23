// src/GitVisualizer/Models/RepoState.cs
namespace GitVisualizer.Models;

public record RepoState(
    bool IsInitialized,
    string? CurrentBranch,
    IReadOnlyList<string> StagedFiles,
    IReadOnlyList<string> UntrackedFiles
);
