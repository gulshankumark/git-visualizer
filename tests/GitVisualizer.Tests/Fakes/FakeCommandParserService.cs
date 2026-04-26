// tests/GitVisualizer.Tests/Fakes/FakeCommandParserService.cs
using GitVisualizer.Models;
using GitVisualizer.Services;

namespace GitVisualizer.Tests.Fakes;

internal sealed class FakeCommandParserService : ICommandParserService
{
    public GitCommand Parse(string rawInput)
    {
        var parts = rawInput.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 2 || parts[0].ToLowerInvariant() != "git")
            return new GitCommand("", new Dictionary<string, string>());
        return new GitCommand(parts[1].ToLowerInvariant(), new Dictionary<string, string>());
    }

    public string? Suggest(string rawInput) => null;
}
