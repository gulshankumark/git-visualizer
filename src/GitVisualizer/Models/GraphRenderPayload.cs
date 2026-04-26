// src/GitVisualizer/Models/GraphRenderPayload.cs
namespace GitVisualizer.Models;

public sealed record GraphRenderPayload(
    string HeadBranch,
    string TrunkBranch,
    IReadOnlyList<RenderCommit> Commits,
    IReadOnlyDictionary<string, string> BranchTips,
    IReadOnlyDictionary<string, string> CommitBranch);

public sealed record RenderCommit(
    string Oid,
    string ShortOid,
    string Message,
    string Author,
    long Timestamp,
    IReadOnlyList<string> Parents,
    string Branch,
    bool IsHead,
    bool IsMerge);
