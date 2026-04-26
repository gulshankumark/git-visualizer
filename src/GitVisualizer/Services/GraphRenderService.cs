// src/GitVisualizer/Services/GraphRenderService.cs
using GitVisualizer.Models;

namespace GitVisualizer.Services;

public sealed class GraphRenderService : IGraphRenderService
{
    public GraphRenderPayload? BuildPayload(RepoState state)
    {
        var graph = state.Graph;
        if (graph is null || !state.IsInitialized || graph.Commits.Count == 0)
            return null;

        var commitToBranch = BuildCommitToBranch(graph);

        // Trunk = main/master if either exists, else the head branch. The first
        // chronological commit always belongs to the trunk after the priority walk.
        var trunkBranch = graph.Commits.Count > 0 && commitToBranch.TryGetValue(graph.Commits[0].Oid, out var t)
            ? t
            : graph.HeadBranch;

        var headTipOid = graph.BranchTips.TryGetValue(graph.HeadBranch, out var tip) ? tip : null;

        var renderCommits = new List<RenderCommit>(graph.Commits.Count);
        foreach (var c in graph.Commits)
        {
            var owningBranch = commitToBranch.TryGetValue(c.Oid, out var ob) ? ob : graph.HeadBranch;
            renderCommits.Add(new RenderCommit(
                c.Oid,
                c.ShortOid,
                c.Message,
                c.Author,
                c.Timestamp,
                c.Parents,
                owningBranch,
                IsHead: headTipOid is not null && c.Oid == headTipOid,
                IsMerge: c.Parents.Count >= 2));
        }

        return new GraphRenderPayload(
            HeadBranch: graph.HeadBranch,
            TrunkBranch: trunkBranch,
            Commits: renderCommits,
            BranchTips: graph.BranchTips,
            CommitBranch: commitToBranch);
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

        // Walk the trunk first (main/master) so shared ancestors are attributed
        // to it instead of whichever feature branch HEAD currently points at.
        // Falls back to HeadBranch when no main/master exists.
        static int Priority(string b, string head)
            => b == "main" ? 0 : b == "master" ? 1 : b == head ? 2 : 3;

        var orderedBranches = graph.BranchTips.Keys
            .OrderBy(b => Priority(b, graph.HeadBranch))
            .ThenBy(b => b, StringComparer.Ordinal)
            .ToList();

        foreach (var branch in orderedBranches)
        {
            if (!graph.BranchTips.TryGetValue(branch, out var tipOid)) continue;

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
}
