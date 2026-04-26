// src/GitVisualizer/Services/SessionStorageService.cs
using GitVisualizer.Models;
using Microsoft.JSInterop;
using System.Text.Json;

namespace GitVisualizer.Services;

public sealed class SessionStorageService : ISessionStorageService
{
    private readonly ILocalStorageService _storage;
    private const int SchemaVersion = 1;
    private const string StateKey = "gitvis.repo";
    private const string PrefsKey = "gitvis.prefs.theme";
    private bool _schemaMismatchDetected;
    
    public SessionStorageService(ILocalStorageService storage)
    {
        _storage = storage;
    }

    public bool SchemaMismatchDetected => _schemaMismatchDetected;

    public async Task<RepoState?> LoadStateAsync()
    {
        _schemaMismatchDetected = false;
        
        try
        {
            var record = _storage.GetItem<SessionStorageRecord<RepoStateDto>>(StateKey);
            
            if (record == null)
                return null;

            if (record.SchemaVersion != SchemaVersion)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[SessionStorageService] Schema version mismatch: expected {SchemaVersion}, got {record.SchemaVersion}. Clearing stale data.");
                _storage.RemoveItem(StateKey);
                _schemaMismatchDetected = true;
                return null;
            }

            if (record.Data == null)
                return null;

            System.Diagnostics.Debug.WriteLine(
                $"[SessionStorageService] Loaded RepoState from localStorage (schema v{record.SchemaVersion})");
            
            return record.Data.ToRepoState();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"[SessionStorageService] Error loading state: {ex.Message}. Returning null.");
            return null;
        }
    }

    public async Task SaveStateAsync(RepoState state)
    {
        try
        {
            var dto = RepoStateDto.FromRepoState(state);
            var record = new SessionStorageRecord<RepoStateDto>(SchemaVersion, dto);
            _storage.SetItem(StateKey, record);
            System.Diagnostics.Debug.WriteLine(
                $"[SessionStorageService] Saved RepoState to localStorage (schema v{SchemaVersion})");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"[SessionStorageService] Error saving state: {ex.Message}");
        }
    }

    public async Task<UserPrefs?> LoadPrefsAsync()
    {
        try
        {
            var theme = _storage.GetItem<string>(PrefsKey);
            return theme != null ? new UserPrefs(theme) : null;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"[SessionStorageService] Error loading prefs: {ex.Message}");
            return null;
        }
    }

    public async Task SavePrefsAsync(UserPrefs prefs)
    {
        try
        {
            _storage.SetItem(PrefsKey, prefs.Theme);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"[SessionStorageService] Error saving prefs: {ex.Message}");
        }
    }

    public async Task ClearAllAsync()
    {
        try
        {
            // Clear all gitvis.* keys from localStorage
            _storage.RemoveItem(StateKey);
            _storage.RemoveItem(PrefsKey);
            System.Diagnostics.Debug.WriteLine("[SessionStorageService] Cleared all gitvis.* keys from localStorage");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"[SessionStorageService] Error clearing storage: {ex.Message}");
            // Don't rethrow — allow reset to continue even if storage clear fails
        }
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        // localStorage doesn't need explicit cleanup
        await ValueTask.CompletedTask;
    }
}

/// <summary>
/// Versioned wrapper for localStorage records. Enables schema migration on future updates.
/// </summary>
public record SessionStorageRecord<T>(int SchemaVersion, T? Data);

/// <summary>
/// DTO for RepoState serialization. Converts IReadOnlyList to List for JSON compatibility.
/// </summary>
public sealed record RepoStateDto(
    bool IsInitialized,
    string? CurrentBranch,
    List<string> StagedFiles,
    List<string> UntrackedFiles,
    CommitGraphDto? Graph = null)
{
    public RepoState ToRepoState() =>
        new(IsInitialized, CurrentBranch, StagedFiles, UntrackedFiles,
            Graph?.ToCommitGraph());

    public static RepoStateDto FromRepoState(RepoState state) =>
        new(state.IsInitialized, state.CurrentBranch,
            state.StagedFiles.ToList(), state.UntrackedFiles.ToList(),
            state.Graph != null ? CommitGraphDto.FromCommitGraph(state.Graph) : null);
}

/// <summary>
/// DTO for CommitGraph serialization.
/// </summary>
public sealed record CommitGraphDto(
    List<CommitNodeDto> Commits,
    Dictionary<string, string> BranchTips,
    string HeadBranch)
{
    public CommitGraph ToCommitGraph() =>
        new(Commits.Select(c => c.ToCommitNode()).ToList(), BranchTips, HeadBranch);

    public static CommitGraphDto FromCommitGraph(CommitGraph graph) =>
        new(
            graph.Commits.Select(CommitNodeDto.FromCommitNode).ToList(),
            new Dictionary<string, string>(graph.BranchTips),
            graph.HeadBranch);
}

/// <summary>
/// DTO for CommitNode serialization.
/// </summary>
public sealed record CommitNodeDto(
    string Oid,
    string ShortOid,
    string Message,
    string Author,
    long Timestamp,
    List<string> Parents,
    string Branch)
{
    public CommitNode ToCommitNode() =>
        new(Oid, ShortOid, Message, Author, Timestamp, Parents, Branch);

    public static CommitNodeDto FromCommitNode(CommitNode node) =>
        new(node.Oid, node.ShortOid, node.Message, node.Author, node.Timestamp,
            node.Parents.ToList(), node.Branch);
}
