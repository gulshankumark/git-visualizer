// src/GitVisualizer/Models/CommandHistoryEntry.cs
namespace GitVisualizer.Models;

public record CommandHistoryEntry(
    string Command,
    CommandResult Result,
    DateTime Timestamp
);
