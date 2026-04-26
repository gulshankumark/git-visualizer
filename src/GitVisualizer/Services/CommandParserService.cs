// src/GitVisualizer/Services/CommandParserService.cs
using GitVisualizer.Models;

namespace GitVisualizer.Services;

public sealed class CommandParserService : ICommandParserService
{
    private static readonly string[] SupportedCommands =
        ["init", "add", "commit", "branch", "checkout", "merge", "help", "log", "status"];

    private static readonly IReadOnlyDictionary<string, string> EmptyArgs =
        new Dictionary<string, string>();

    public GitCommand Parse(string rawInput)
    {
        var trimmed = rawInput.Trim();
        if (string.IsNullOrEmpty(trimmed))
            return new GitCommand("", EmptyArgs);

        var parts = trimmed.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts[0].ToLowerInvariant() != "git" || parts.Length < 2)
            return new GitCommand("", EmptyArgs);

        var name = parts[1].ToLowerInvariant();
        var args = parts.Length > 2 ? ParseArgs(parts, 2) : EmptyArgs;
        return new GitCommand(name, args);
    }

    public string? Suggest(string rawInput)
    {
        var parts = rawInput.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 2 || parts[0].ToLowerInvariant() != "git")
            return null;

        var token = parts[1].ToLowerInvariant();
        if (Array.IndexOf(SupportedCommands, token) >= 0)
            return null; // exact match — not a typo

        var (best, distance) = SupportedCommands
            .Select(cmd => (cmd, dist: Levenshtein(token, cmd)))
            .MinBy(x => x.dist);

        return distance <= 2 ? $"git {best}" : null;
    }

    private static IReadOnlyDictionary<string, string> ParseArgs(string[] parts, int start)
    {
        var args = new Dictionary<string, string>();
        int positional = 0;
        int i = start;
        while (i < parts.Length)
        {
            if (parts[i].StartsWith('-') && parts[i].Length > 1)
            {
                var flag = parts[i][1..].ToLowerInvariant();
                if (flag == "m" && i + 1 < parts.Length)
                {
                    // -m consumes the remainder of parts, stripping surrounding quotes
                    args["m"] = string.Join(" ", parts[(i + 1)..]).Trim('"', '\'');
                    break;
                }
                else if (flag == "b" && i + 1 < parts.Length)
                {
                    args["b"] = parts[i + 1];
                    i += 2;
                }
                else
                {
                    args[flag] = "";
                    i++;
                }
            }
            else
            {
                args[$"arg{positional}"] = parts[i];
                positional++;
                i++;
            }
        }
        return args;
    }

    private static int Levenshtein(string a, string b)
    {
        int[,] dp = new int[a.Length + 1, b.Length + 1];
        for (int i = 0; i <= a.Length; i++) dp[i, 0] = i;
        for (int j = 0; j <= b.Length; j++) dp[0, j] = j;
        for (int i = 1; i <= a.Length; i++)
            for (int j = 1; j <= b.Length; j++)
                dp[i, j] = a[i - 1] == b[j - 1]
                    ? dp[i - 1, j - 1]
                    : 1 + Math.Min(dp[i - 1, j - 1], Math.Min(dp[i - 1, j], dp[i, j - 1]));
        return dp[a.Length, b.Length];
    }
}
