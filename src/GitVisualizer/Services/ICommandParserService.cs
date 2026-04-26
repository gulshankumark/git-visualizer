// src/GitVisualizer/Services/ICommandParserService.cs
using GitVisualizer.Models;

namespace GitVisualizer.Services;

public interface ICommandParserService
{
    /// <summary>
    /// Parse raw input into a normalised <see cref="GitCommand"/>.
    /// <c>Name</c> is empty string if input is not a git command or has no subcommand.
    /// Input is normalised to lowercase before processing.
    /// </summary>
    GitCommand Parse(string rawInput);

    /// <summary>
    /// Return the nearest valid git command suggestion for a typo (Levenshtein distance ≤ 2),
    /// or <c>null</c> if the input subcommand is already valid or no close match exists.
    /// </summary>
    string? Suggest(string rawInput);
}
