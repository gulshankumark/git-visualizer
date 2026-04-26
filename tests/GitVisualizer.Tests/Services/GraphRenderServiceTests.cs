// tests/GitVisualizer.Tests/Services/GraphRenderServiceTests.cs
using GitVisualizer.Models;
using GitVisualizer.Services;

namespace GitVisualizer.Tests.Services;

public class GraphRenderServiceTests
{
    private readonly GraphRenderService _sut = new();

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static CommitGraph SingleBranchGraph(int commitCount = 2)
    {
        var commits = new List<CommitNode>();
        string? prevOid = null;
        for (var i = 0; i < commitCount; i++)
        {
            var oid = $"oid{i:D3}0000000000000000000000000000000000000000".Substring(0, 40);
            var parents = prevOid is not null ? new List<string> { prevOid } : new List<string>();
            commits.Add(new CommitNode(oid, oid[..7], $"Commit {i}", "Learner", 1000 + i, parents, "main"));
            prevOid = oid;
        }
        var tips = new Dictionary<string, string> { ["main"] = commits.Last().Oid };
        return new CommitGraph(commits, tips, "main");
    }

    private static CommitGraph TwoBranchMergeGraph()
    {
        // main: A → B
        // feature: A → C
        // merge commit: D (parents: B, C) on main
        var a = new CommitNode("aaaa" + new string('0', 36), "aaaa000", "Initial commit", "Learner", 1000, [], "main");
        var b = new CommitNode("bbbb" + new string('0', 36), "bbbb000", "Second commit", "Learner", 1001, [a.Oid], "main");
        var c = new CommitNode("cccc" + new string('0', 36), "cccc000", "Feature work", "Learner", 1002, [a.Oid], "feature");
        var d = new CommitNode("dddd" + new string('0', 36), "dddd000", "Merge feature", "Learner", 1003, [b.Oid, c.Oid], "main");
        var tips = new Dictionary<string, string> { ["main"] = d.Oid, ["feature"] = c.Oid };
        return new CommitGraph([a, b, c, d], tips, "main");
    }

    // ── ToMermaidSyntax ───────────────────────────────────────────────────────

    [Fact]
    public void ToMermaidSyntax_NullGraph_ReturnsEmptyString()
    {
        var state = new RepoState(true, "main", [], []);
        Assert.Equal(string.Empty, _sut.ToMermaidSyntax(state));
    }

    [Fact]
    public void ToMermaidSyntax_EmptyGraph_ReturnsEmptyString()
    {
        var state = new RepoState(true, "main", [], [], CommitGraph.Empty);
        Assert.Equal(string.Empty, _sut.ToMermaidSyntax(state));
    }

    [Fact]
    public void ToMermaidSyntax_NotInitialized_ReturnsEmptyString()
    {
        var state = new RepoState(false, "main", [], [], SingleBranchGraph());
        Assert.Equal(string.Empty, _sut.ToMermaidSyntax(state));
    }

    [Fact]
    public void ToMermaidSyntax_SingleBranch_ContainsGitGraphDirective()
    {
        var state = new RepoState(true, "main", [], [], SingleBranchGraph());
        var syntax = _sut.ToMermaidSyntax(state);
        Assert.Contains("gitGraph LR:", syntax);
    }

    [Fact]
    public void ToMermaidSyntax_SingleBranch_ContainsMainBranchInHeader()
    {
        var state = new RepoState(true, "main", [], [], SingleBranchGraph());
        var syntax = _sut.ToMermaidSyntax(state);
        Assert.Contains("mainBranchName", syntax);
        Assert.Contains("main", syntax);
    }

    [Fact]
    public void ToMermaidSyntax_SingleBranch_ContainsCommitKeyword()
    {
        var state = new RepoState(true, "main", [], [], SingleBranchGraph(3));
        var syntax = _sut.ToMermaidSyntax(state);
        Assert.Contains("commit id:", syntax);
    }

    [Fact]
    public void ToMermaidSyntax_TwoBranchesWithMerge_ContainsMergeKeyword()
    {
        var state = new RepoState(true, "main", [], [], TwoBranchMergeGraph());
        var syntax = _sut.ToMermaidSyntax(state);
        Assert.Contains("merge ", syntax);
    }

    [Fact]
    public void ToMermaidSyntax_TwoBranchesWithMerge_ContainsBranchDirective()
    {
        var state = new RepoState(true, "main", [], [], TwoBranchMergeGraph());
        var syntax = _sut.ToMermaidSyntax(state);
        Assert.Contains("branch feature", syntax);
    }

    [Fact]
    public void ToMermaidSyntax_LongCommitMessage_TruncatedTo40Chars()
    {
        var longMessage = new string('A', 60);
        var commit = new CommitNode("aaaa" + new string('0', 36), "aaaa000", longMessage, "Learner", 1000, [], "main");
        var graph = new CommitGraph([commit], new Dictionary<string, string> { ["main"] = commit.Oid }, "main");
        var state = new RepoState(true, "main", [], [], graph);
        var syntax = _sut.ToMermaidSyntax(state);
        // Ensure no more than 40 A's appear in message part
        Assert.DoesNotContain(new string('A', 41), syntax);
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
        Assert.Contains("no commits", label, StringComparison.OrdinalIgnoreCase);
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
    public void BuildAriaLabel_MultipleBranches_IncludesBranchCount()
    {
        var state = new RepoState(true, "main", [], [], TwoBranchMergeGraph());
        var label = _sut.BuildAriaLabel(state);
        Assert.Contains("2", label); // 2 branches total
    }
}
