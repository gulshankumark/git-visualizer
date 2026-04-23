// src/GitVisualizer/Models/CommandResult.cs
namespace GitVisualizer.Models;

public record CommandResult(
    bool Success,
    string Output,
    string? ErrorMessage,
    string? SuggestedFix,
    RepoState? UpdatedState
);
