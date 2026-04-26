// src/GitVisualizer/Services/GraphRenderService.cs
using System.Text;
using GitVisualizer.Models;

namespace GitVisualizer.Services;

public sealed class GraphRenderService : IGraphRenderService
{
    public string ToMermaidSyntax(RepoState state)
    {
        var graph = state.Graph;
        if (graph is null || !state.IsInitialized || graph.Commits.Count == 0)
            return string.Empty;

        var sb = new StringBuilder();
        sb.AppendLine($"%%{{init: {{'gitGraph': {{'mainBranchName': '{EscapeId(graph.HeadBranch)}', 'rotateCommitLabel': false}}}}}}%%");
        sb.AppendLine("gitGraph LR:");

        // Build commit-to-branch ownership via first-parent walk
        var commitToBranch = BuildCommitToBranch(graph);

        // Emit commits sorted oldest-first (already sorted by gitGetGraph)
        string? lastBranch = null;

        foreach (var commit in graph.Commits)
        {
            var branch = commitToBranch.TryGetValue(commit.Oid, out var b) ? b : graph.HeadBranch;

            // Emit branch switch if needed
            if (lastBranch is null)
            {
                // First commit — always on HeadBranch; no explicit checkout needed
                lastBranch = branch;
            }
            else if (branch != lastBranch)
            {
                sb.AppendLine($"   branch {EscapeId(branch)}");
                sb.AppendLine($"   checkout {EscapeId(branch)}");
                lastBranch = branch;
            }

            if (commit.Parents.Count >= 2)
            {
                // Merge commit — find the source branch from second parent
                var sourceOid = commit.Parents[1];
                var sourceBranch = commitToBranch.TryGetValue(sourceOid, out var sb2) ? sb2 : "unknown";
                // Checkout the target branch first if we were on a different branch
                var targetBranch = commitToBranch.TryGetValue(commit.Parents[0], out var tb) ? tb : graph.HeadBranch;
                if (lastBranch != targetBranch)
                {
                    sb.AppendLine($"   checkout {EscapeId(targetBranch)}");
                    lastBranch = targetBranch;
                }
                sb.AppendLine($"   merge {EscapeId(sourceBranch)} id: \"{EscapeId(commit.ShortOid)}\"");
            }
            else
            {
                var msg = EscapeMessage(commit.Message);
                sb.AppendLine($"   commit id: \"{EscapeId(commit.ShortOid)}\" msg: \"{msg}\"");
            }
        }

        return sb.ToString().TrimEnd();
    }

    public string BuildAriaLabel(RepoState? state)
    {
        if (state is null || !state.IsInitialized)
            return "Commit graph: no repository initialised.";

        var graph = state.Graph;
        if (graph is null || graph.Commits.Count == 0)
            return $"Commit graph: repository initialised on branch {state.CurrentBranch ?? "main"}, no commits yet.";

        var branchCount = graph.BranchTips.Count;
        var commitCount = graph.Commits.Count;
        var branchWord = branchCount == 1 ? "branch" : "branches";
        return $"Commit graph: {state.CurrentBranch ?? graph.HeadBranch} branch, {commitCount} {(commitCount == 1 ? "commit" : "commits")}. {branchCount} {branchWord} total.";
    }

    private static Dictionary<string, string> BuildCommitToBranch(CommitGraph graph)
    {
        var result = new Dictionary<string, string>();

        // Process HeadBranch first so it gets first-parent priority
        var orderedBranches = graph.BranchTips.Keys
            .OrderBy(b => b == graph.HeadBranch ? 0 : 1)
            .ToList();

        foreach (var branch in orderedBranches)
        {
            if (!graph.BranchTips.TryGetValue(branch, out var tipOid)) continue;

            // Walk first-parent chain from tip
            var current = tipOid;
            var visited = new HashSet<string>();

            while (current is not null && !result.ContainsKey(current) && !visited.Contains(current))
            {
                visited.Add(current);
                result[current] = branch;

                var node = graph.Commits.FirstOrDefault(c => c.Oid == current);
                current = node?.Parents.Count > 0 ? node.Parents[0] : null!;
            }
        }

        return result;
    }

    private static string EscapeId(string id)
        => id.Replace("\"", "'").Replace(" ", "-");

    private static string EscapeMessage(string message)
    {
        var truncated = message.Length > 40 ? message[..40] : message;
        return truncated.Replace("\"", "'");
    }
}
