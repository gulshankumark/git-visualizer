// tests/GitVisualizer.Tests/Services/GraphRenderServiceTests.cs
using GitVisualizer.Models;
using GitVisualizer.Services;

namespace GitVisualizer.Tests.Services;

public class GraphRenderServiceTests
{
    private readonly GraphRenderService _sut = new();

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static CommitGraph SingleBranchGraph(int commitCount = 2, string branch = "main")
    {
        var commits = new List<CommitNode>();
        string? prevOid = null;
        for (var i = 0; i < commitCount; i++)
        {
            var oid = $"oid{i:D3}0000000000000000000000000000000000000000".Substring(0, 40);
            var parents = prevOid is not null ? new List<string> { prevOid } : new List<string>();
            commits.Add(new CommitNode(oid, oid[..7], $"Commit {i}", "Learner", 1000 + i, parents, branch));
            prevOid = oid;
        }
        var tips = new Dictionary<string, string> { [branch] = commits.Last().Oid };
        return new CommitGraph(commits, tips, branch);
    }

    private static CommitGraph TwoBranchMergeGraph()
    {
        // main:    A → B ─────┐
        // feature:     A → C ─┘ → D (merge)
        var a = new CommitNode("aaaa" + new string('0', 36), "aaaa000", "Initial commit", "Learner", 1000, [], "main");
        var b = new CommitNode("bbbb" + new string('0', 36), "bbbb000", "Second commit",  "Learner", 1001, [a.Oid], "main");
        var c = new CommitNode("cccc" + new string('0', 36), "cccc000", "Feature work",   "Learner", 1002, [a.Oid], "feature");
        var d = new CommitNode("dddd" + new string('0', 36), "dddd000", "Merge feature",  "Learner", 1003, [b.Oid, c.Oid], "main");
        var tips = new Dictionary<string, string> { ["main"] = d.Oid, ["feature"] = c.Oid };
        return new CommitGraph([a, b, c, d], tips, "main");
    }

    /// <summary>
    /// Reproduces the bug from screenshots 2.incorrect-history.png:
    ///   master: A → B ──────── C ──────── E
    ///   abc:        ↘  D ────────  F
    /// HEAD ends on abc. Shared ancestors A, B must still be attributed to master.
    /// </summary>
    private static CommitGraph DivergedHeadOnFeatureGraph()
    {
        var a = new CommitNode("a" + new string('0', 39), "a000000", "A", "L", 1000, [], "master");
        var b = new CommitNode("b" + new string('0', 39), "b000000", "B", "L", 1001, [a.Oid], "master");
        var d = new CommitNode("d" + new string('0', 39), "d000000", "D", "L", 1002, [b.Oid], "abc");
        var c = new CommitNode("c" + new string('0', 39), "c000000", "C", "L", 1003, [b.Oid], "master");
        var f = new CommitNode("f" + new string('0', 39), "f000000", "F", "L", 1004, [d.Oid], "abc");
        var e = new CommitNode("e" + new string('0', 39), "e000000", "E", "L", 1005, [c.Oid], "master");
        var tips = new Dictionary<string, string> { ["master"] = e.Oid, ["abc"] = f.Oid };
        return new CommitGraph([a, b, d, c, f, e], tips, "abc");
    }

    // ── BuildPayload ──────────────────────────────────────────────────────────

    [Fact]
    public void BuildPayload_NullGraph_ReturnsNull()
    {
        var state = new RepoState(true, "main", [], []);
        Assert.Null(_sut.BuildPayload(state));
    }

    [Fact]
    public void BuildPayload_EmptyGraph_ReturnsNull()
    {
        var state = new RepoState(true, "main", [], [], CommitGraph.Empty);
        Assert.Null(_sut.BuildPayload(state));
    }

    [Fact]
    public void BuildPayload_NotInitialized_ReturnsNull()
    {
        var state = new RepoState(false, "main", [], [], SingleBranchGraph());
        Assert.Null(_sut.BuildPayload(state));
    }

    [Fact]
    public void BuildPayload_SingleBranch_AssignsAllCommitsToHeadBranch()
    {
        var state = new RepoState(true, "main", [], [], SingleBranchGraph(3));
        var payload = _sut.BuildPayload(state);
        Assert.NotNull(payload);
        Assert.Equal("main", payload!.HeadBranch);
        Assert.Equal("main", payload.TrunkBranch);
        Assert.All(payload.Commits, c => Assert.Equal("main", c.Branch));
    }

    [Fact]
    public void BuildPayload_FlagsHeadCommitOnHeadBranchTip()
    {
        var state = new RepoState(true, "main", [], [], SingleBranchGraph(3));
        var payload = _sut.BuildPayload(state)!;
        var headCount = payload.Commits.Count(c => c.IsHead);
        Assert.Equal(1, headCount);
        Assert.True(payload.Commits[^1].IsHead, "HEAD should be the chronologically-last commit on the single branch");
    }

    [Fact]
    public void BuildPayload_TwoBranches_TrunkAndForkAttribution()
    {
        var state = new RepoState(true, "main", [], [], TwoBranchMergeGraph());
        var payload = _sut.BuildPayload(state)!;

        Assert.Equal("main", payload.TrunkBranch);
        // A and B (shared ancestors / main-only) and D (merge tip on main) belong to main.
        Assert.Equal("main",    payload.CommitBranch[payload.Commits[0].Oid]); // A
        Assert.Equal("main",    payload.CommitBranch[payload.Commits[1].Oid]); // B
        Assert.Equal("feature", payload.CommitBranch[payload.Commits[2].Oid]); // C
        Assert.Equal("main",    payload.CommitBranch[payload.Commits[3].Oid]); // D
    }

    [Fact]
    public void BuildPayload_TwoBranches_FlagsMergeCommit()
    {
        var state = new RepoState(true, "main", [], [], TwoBranchMergeGraph());
        var payload = _sut.BuildPayload(state)!;
        var merges = payload.Commits.Where(c => c.IsMerge).ToList();
        Assert.Single(merges);
        Assert.Equal(2, merges[0].Parents.Count);
    }

    [Fact]
    public void BuildPayload_HeadOnFeatureBranch_TrunkStillAttributesSharedAncestors()
    {
        // Regression: when HEAD is on a non-trunk branch, shared ancestors must
        // still be attributed to master (not to the feature branch).
        var state = new RepoState(true, "abc", [], [], DivergedHeadOnFeatureGraph());
        var payload = _sut.BuildPayload(state)!;

        Assert.Equal("master", payload.TrunkBranch);
        Assert.Equal("master", payload.CommitBranch[payload.Commits[0].Oid]); // A
        Assert.Equal("master", payload.CommitBranch[payload.Commits[1].Oid]); // B
        Assert.Equal("abc",    payload.CommitBranch.Values.First(v => v == "abc"));
        // Exactly two abc commits (D, F)
        Assert.Equal(2, payload.CommitBranch.Values.Count(v => v == "abc"));
        // Exactly four master commits (A, B, C, E)
        Assert.Equal(4, payload.CommitBranch.Values.Count(v => v == "master"));
    }

    [Fact]
    public void BuildPayload_HeadOnFeatureBranch_HeadFlagOnFeatureTip()
    {
        var state = new RepoState(true, "abc", [], [], DivergedHeadOnFeatureGraph());
        var payload = _sut.BuildPayload(state)!;
        var head = Assert.Single(payload.Commits, c => c.IsHead);
        Assert.Equal("abc", head.Branch);
        Assert.Equal("f0000000000000000000000000000000000000000".Substring(0, 40), head.Oid);
    }

    [Fact]
    public void BuildPayload_PreservesChronologicalOrder()
    {
        var state = new RepoState(true, "main", [], [], TwoBranchMergeGraph());
        var payload = _sut.BuildPayload(state)!;
        var timestamps = payload.Commits.Select(c => c.Timestamp).ToList();
        Assert.Equal(timestamps.OrderBy(t => t).ToList(), timestamps);
    }

    [Fact]
    public void BuildPayload_BranchTipsRoundTrip()
    {
        var state = new RepoState(true, "main", [], [], TwoBranchMergeGraph());
        var payload = _sut.BuildPayload(state)!;
        Assert.Equal(2, payload.BranchTips.Count);
        Assert.Contains("main", payload.BranchTips.Keys);
        Assert.Contains("feature", payload.BranchTips.Keys);
    }

    // ── BuildAriaLabel ────────────────────────────────────────────────────────

    [Fact]
    public void BuildAriaLabel_NullState_ReturnsDefaultLabel()
    {
        var label = _sut.BuildAriaLabel(null);
        Assert.Contains("no repository", label, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void BuildAriaLabel_NotInitialized_ReturnsDefaultLabel()
    {
        var state = new RepoState(false, "main", [], []);
        var label = _sut.BuildAriaLabel(state);
        Assert.Contains("no repository", label, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void BuildAriaLabel_InitializedNoCommits_MentionsNoCommits()
    {
        var state = new RepoState(true, "main", [], [], CommitGraph.Empty);
        var label = _sut.BuildAriaLabel(state);
        Assert.Contains("0 commits", label, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void BuildAriaLabel_InitializedWithCommits_IncludesBranchAndCount()
    {
        var state = new RepoState(true, "main", [], [], SingleBranchGraph(3));
        var label = _sut.BuildAriaLabel(state);
        Assert.Contains("main", label);
        Assert.Contains("3", label);
    }

    [Fact]
    public void BuildAriaLabel_MultipleBranches_IncludesCommitCount()
    {
        var state = new RepoState(true, "main", [], [], TwoBranchMergeGraph());
        var label = _sut.BuildAriaLabel(state);
        // TwoBranchMergeGraph has 4 commits; label should reflect the total count
        Assert.Contains("4", label);
        Assert.Contains("main", label);
    }
}
