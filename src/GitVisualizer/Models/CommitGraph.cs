// src/GitVisualizer/Models/CommitGraph.cs
using System.Text.Json;

namespace GitVisualizer.Models;

public sealed record CommitNode(
    string Oid,
    string ShortOid,
    string Message,
    string Author,
    long Timestamp,
    IReadOnlyList<string> Parents,
    string Branch);

public sealed record CommitGraph(
    IReadOnlyList<CommitNode> Commits,
    IReadOnlyDictionary<string, string> BranchTips,
    string HeadBranch)
{
    public static CommitGraph Empty { get; } =
        new([], new Dictionary<string, string>(), "main");

    public static CommitGraph FromJson(JsonElement json)
    {
        var commits = new List<CommitNode>();
        if (json.TryGetProperty("commits", out var commitsArray))
        {
            foreach (var c in commitsArray.EnumerateArray())
            {
                if (c.TryGetProperty("oid", out var oid) &&
                    c.TryGetProperty("shortOid", out var shortOid) &&
                    c.TryGetProperty("message", out var message) &&
                    c.TryGetProperty("author", out var author) &&
                    c.TryGetProperty("timestamp", out var timestamp) &&
                    c.TryGetProperty("parents", out var parentsArray) &&
                    c.TryGetProperty("branch", out var branch))
                {
                    var parents = parentsArray.EnumerateArray()
                        .Select(p => p.GetString() ?? "").ToList();
                    commits.Add(new CommitNode(
                        oid.GetString() ?? "",
                        shortOid.GetString() ?? "",
                        message.GetString() ?? "",
                        author.GetString() ?? "",
                        timestamp.GetInt64(),
                        parents,
                        branch.GetString() ?? ""));
                }
            }
        }

        var tips = new Dictionary<string, string>();
        if (json.TryGetProperty("branchTips", out var branchTips))
        {
            foreach (var p in branchTips.EnumerateObject())
            {
                tips[p.Name] = p.Value.GetString() ?? "";
            }
        }

        var head = "main";
        if (json.TryGetProperty("headBranch", out var headBranch))
        {
            head = headBranch.GetString() ?? "main";
        }

        return new CommitGraph(commits, tips, head);
    }
}
