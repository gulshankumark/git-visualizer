// tests/GitVisualizer.Tests/Services/SessionStorageServiceTests.cs
using GitVisualizer.Services;
using GitVisualizer.Models;

namespace GitVisualizer.Tests.Services;

public class SessionStorageServiceTests
{
    [Fact]
    public void StateSerializationRoundTrip_WithCompleteRepoState_PreservesAllFields()
    {
        // Test the DTO serialization/deserialization logic
        var originalState = new RepoState(
            IsInitialized: true,
            CurrentBranch: "feature/test",
            StagedFiles: new[] { "file1.txt", "file2.md" },
            UntrackedFiles: new[] { "new.txt" },
            Graph: new CommitGraph(
                Commits: new[]
                {
                    new CommitNode("abc123", "abc", "Initial commit", "John Doe", 1234567890,
                        new[] { "def456" }, "main")
                },
                BranchTips: new Dictionary<string, string> { { "main", "abc123" } },
                HeadBranch: "main"
            )
        );

        // Convert to DTO and back
        var dto = RepoStateDto.FromRepoState(originalState);
        var restoredState = dto.ToRepoState();

        Assert.NotNull(restoredState);
        Assert.Equal(originalState.IsInitialized, restoredState.IsInitialized);
        Assert.Equal(originalState.CurrentBranch, restoredState.CurrentBranch);
        Assert.Equal(originalState.StagedFiles.Count, restoredState.StagedFiles.Count);
        Assert.Equal(originalState.UntrackedFiles.Count, restoredState.UntrackedFiles.Count);
        Assert.NotNull(restoredState.Graph);
        Assert.Equal(originalState.Graph.HeadBranch, restoredState.Graph.HeadBranch);
        Assert.Single(restoredState.Graph.Commits);
    }

    [Fact]
    public void StateSerializationRoundTrip_WithEmptyState_PreservesAllFields()
    {
        var originalState = new RepoState(
            IsInitialized: false,
            CurrentBranch: null,
            StagedFiles: new string[0],
            UntrackedFiles: new string[0],
            Graph: null
        );

        var dto = RepoStateDto.FromRepoState(originalState);
        var restoredState = dto.ToRepoState();

        Assert.NotNull(restoredState);
        Assert.False(restoredState.IsInitialized);
        Assert.Null(restoredState.CurrentBranch);
        Assert.Empty(restoredState.StagedFiles);
        Assert.Empty(restoredState.UntrackedFiles);
        Assert.Null(restoredState.Graph);
    }

    [Fact]
    public void CommitNodeSerializationRoundTrip_PreservesAllFields()
    {
        var original = new CommitNode(
            "abc123def456",
            "abc123",
            "Feature: implement persistence",
            "Jane Developer",
            1704067200,
            new[] { "parent1", "parent2" },
            "main"
        );

        var dto = CommitNodeDto.FromCommitNode(original);
        var restored = dto.ToCommitNode();

        Assert.Equal(original.Oid, restored.Oid);
        Assert.Equal(original.ShortOid, restored.ShortOid);
        Assert.Equal(original.Message, restored.Message);
        Assert.Equal(original.Author, restored.Author);
        Assert.Equal(original.Timestamp, restored.Timestamp);
        Assert.Equal(original.Parents.Count, restored.Parents.Count);
        Assert.Equal(original.Branch, restored.Branch);
    }

    [Fact]
    public void CommitGraphSerializationRoundTrip_WithMultipleCommits_PreservesCommits()
    {
        var original = new CommitGraph(
            Commits: new[]
            {
                new CommitNode("aaa", "aaa", "First", "Alice", 1000, new[] { "bbb" }, "main"),
                new CommitNode("bbb", "bbb", "Second", "Bob", 2000, new string[0], "main")
            },
            BranchTips: new Dictionary<string, string>
            {
                { "main", "aaa" }
            },
            HeadBranch: "main"
        );

        var dto = CommitGraphDto.FromCommitGraph(original);
        var restored = dto.ToCommitGraph();

        Assert.NotNull(restored);
        Assert.Equal(2, restored.Commits.Count);
        Assert.Equal("main", restored.HeadBranch);
        Assert.NotEmpty(restored.BranchTips);
    }

    [Fact]
    public void SessionStorageRecord_WithValidData_StoresAndRetrievesVersion()
    {
        var dto = new RepoStateDto(true, "main", new(), new(), null);
        var record = new SessionStorageRecord<RepoStateDto>(1, dto);

        Assert.Equal(1, record.SchemaVersion);
        Assert.NotNull(record.Data);
        Assert.True(record.Data.IsInitialized);
    }

    [Fact]
    public void SessionStorageRecord_WithNullData_StoresVersion()
    {
        var record = new SessionStorageRecord<RepoStateDto>(99, null);

        Assert.Equal(99, record.SchemaVersion);
        Assert.Null(record.Data);
    }

    [Fact]
    public void UserPrefsRecord_WithThemeValue_StoresCorrectly()
    {
        var prefs = new UserPrefs("dark");

        Assert.Equal("dark", prefs.Theme);
    }
}

