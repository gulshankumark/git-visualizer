// src/GitVisualizer/Models/GitCommand.cs
namespace GitVisualizer.Models;

/// <summary>Parsed git command with a normalised lowercase name and extracted arguments.</summary>
/// <param name="Name">Lowercase git subcommand (e.g. "commit", "init"). Empty string if input is not a git command.</param>
/// <param name="Args">
/// Parsed arguments keyed by flag letter (e.g. "m", "b") or positional index ("arg0", "arg1").
/// </param>
public record GitCommand(string Name, IReadOnlyDictionary<string, string> Args);
