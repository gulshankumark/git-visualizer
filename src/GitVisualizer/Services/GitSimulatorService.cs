// src/GitVisualizer/Services/GitSimulatorService.cs
using GitVisualizer.Interop;
using GitVisualizer.Models;
using Microsoft.JSInterop;
using System.Text.Json;

namespace GitVisualizer.Services;

public sealed class GitSimulatorService : IGitSimulatorService
{
    private readonly GitJsInterop _gitJs;
    private readonly ICommandParserService _commandParser;
    private readonly ISessionStorageService _sessionStorage;
    private readonly List<CommandHistoryEntry> _history = new();
    private readonly object _stateLock = new();
    private RepoState? _currentState;
    private bool _isProcessing;
    private bool _sessionRestored;
    private bool _schemaMismatch;

    public GitSimulatorService(
        GitJsInterop gitJs,
        ICommandParserService commandParser,
        ISessionStorageService sessionStorage)
    {
        _gitJs = gitJs;
        _commandParser = commandParser;
        _sessionStorage = sessionStorage;
        _isProcessing = false;
    }

    public bool IsProcessing => _isProcessing;
    public IReadOnlyList<CommandHistoryEntry> CommandHistory => _history;
    public RepoState? CurrentState
    {
        get
        {
            lock (_stateLock)
            {
                return _currentState;
            }
        }
    }
    public bool SessionRestored => _sessionRestored;
    public bool SchemaMismatch => _schemaMismatch;
    public event Action? StateChanged;

    public async Task InitializeAsync()
    {
        try
        {
            var savedState = await _sessionStorage.LoadStateAsync();
            if (savedState != null)
            {
                lock (_stateLock)
                {
                    _currentState = savedState;
                    _sessionRestored = true;
                }
                System.Diagnostics.Debug.WriteLine("[GitSimulatorService] Session restored from localStorage");
                StateChanged?.Invoke();
            }
            
            // Check if schema mismatch was detected during load
            _schemaMismatch = _sessionStorage.SchemaMismatchDetected;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"[GitSimulatorService] Error initializing from session: {ex.Message}");
        }
    }

    public async Task SetSchemaMismatchFlagAsync()
    {
        _schemaMismatch = true;
        await Task.CompletedTask;
    }

    public async Task<CommandResult> ExecuteCommandAsync(string rawCommand)
    {
        if (_isProcessing)
            return new CommandResult(false, "", "A command is already running.", null, null);

        _isProcessing = true;
        StateChanged?.Invoke();

        CommandResult result;
        try
        {
            result = await DispatchCommandAsync(rawCommand.Trim());
            _history.Add(new CommandHistoryEntry(rawCommand, result, DateTime.UtcNow));
        }
        finally
        {
            _isProcessing = false;
            await TryUpdateGraphAsync();
            
            // Save state to session storage after command completes
            lock (_stateLock)
            {
                if (_currentState != null)
                {
                    _ = _sessionStorage.SaveStateAsync(_currentState);
                }
            }
            
            StateChanged?.Invoke();
        }
        return result;
    }

    private Task<CommandResult> DispatchCommandAsync(string command)
    {
        var parts = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0)
            return Task.FromResult(new CommandResult(false, "", "Empty command.", null, null));

        // Guard: non-git input
        if (parts[0].ToLowerInvariant() != "git")
            return Task.FromResult(new CommandResult(false, "",
                $"Unknown command '{command}'. Git commands start with 'git'. Type 'git help' for available commands.",
                "git help", null));

        var gitCommand = _commandParser.Parse(command);

        return gitCommand.Name switch
        {
            "init"     => InitRepoAsync(),
            "add"      => AddAsync(gitCommand.Args.GetValueOrDefault("arg0", ".")),
            "commit"   => CommitAsync(gitCommand.Args.GetValueOrDefault("m", "Update")),
            "branch"   => gitCommand.Args.TryGetValue("arg0", out var bn)
                             ? CreateBranchAsync(bn)
                             : Task.FromResult(new CommandResult(true,
                                 "Branch listing is not yet available. Use 'git log' to see commit history, or 'git branch <name>' to create a branch.",
                                 null, null, _currentState)),
            "checkout" => gitCommand.Args.TryGetValue("b", out var nb)
                             ? CheckoutAsync(nb, createBranch: true)
                             : CheckoutAsync(gitCommand.Args.GetValueOrDefault("arg0", "main")),
            "merge"    => gitCommand.Args.TryGetValue("arg0", out var mb)
                             ? MergeAsync(mb)
                             : Task.FromResult(new CommandResult(false, "", "'git merge' requires a branch name.", null, null)),
            "log"      => GetLogAsync(),
            "status"   => Task.FromResult(BuildStatusResult()),
            "rebase"   => Task.FromResult(new CommandResult(false, "",
                             "'git rebase' is coming in v1.0 — for now try 'git merge'.", "git merge", null)),
            "help"     => Task.FromResult(new CommandResult(true, HelpText, null, null, _currentState)),
            _          => DispatchUnknownCommandAsync(command)
        };
    }

    private Task<CommandResult> DispatchUnknownCommandAsync(string command)
    {
        var suggestion = _commandParser.Suggest(command);
        var errMsg = suggestion is not null
            ? $"'{command}' is not a valid git command. Did you mean '{suggestion}'? Type 'git help' for available commands."
            : $"'{command}' is not a supported git command. Type 'git help' for available commands.";
        return Task.FromResult(new CommandResult(false, "", errMsg, suggestion ?? "git help", null));
    }

    public async Task<CommandResult> InitRepoAsync()
    {
        try
        {
            var json = await _gitJs.GitInitAsync();
            var msg = json.GetProperty("message").GetString() ?? "Initialized.";
            _currentState = new RepoState(true, "main", [], []);
            return new CommandResult(true, msg, null, null, _currentState);
        }
        catch (JSException ex)
        {
            return new CommandResult(false, "", $"Git engine error: {ex.Message}", null, null);
        }
    }

    public async Task<CommandResult> AddAsync(string filepath = ".")
    {
        try
        {
            var json = await _gitJs.GitAddAsync(filepath);
            var msg = json.GetProperty("message").GetString() ?? "Changes staged.";
            return new CommandResult(true, msg, null, null, _currentState);
        }
        catch (JSException ex)
        {
            return new CommandResult(false, "", $"Git engine error: {ex.Message}", null, null);
        }
    }

    public async Task<CommandResult> CommitAsync(string message)
    {
        try
        {
            var json = await _gitJs.GitCommitAsync(message);
            var msg = json.GetProperty("message").GetString() ?? "Committed.";
            return new CommandResult(true, msg, null, null, _currentState);
        }
        catch (JSException ex)
        {
            var err = ex.Message.Contains("Nothing to commit", StringComparison.OrdinalIgnoreCase)
                ? "Nothing to commit. Run 'git add .' first."
                : $"Git engine error: {ex.Message}";
            return new CommandResult(false, "", err, "git add .", null);
        }
    }

    public async Task<CommandResult> CreateBranchAsync(string name)
    {
        try
        {
            var json = await _gitJs.GitBranchAsync(name);
            var msg = json.GetProperty("message").GetString() ?? $"Created branch '{name}'.";
            return new CommandResult(true, msg, null, null, _currentState);
        }
        catch (JSException ex)
        {
            return new CommandResult(false, "", $"Git engine error: {ex.Message}", null, null);
        }
    }

    public async Task<CommandResult> CheckoutAsync(string @ref, bool createBranch = false)
    {
        try
        {
            var json = await _gitJs.GitCheckoutAsync(@ref, createBranch);
            var msg = json.GetProperty("message").GetString() ?? $"Switched to branch '{@ref}'.";
            _currentState = _currentState is not null
                ? _currentState with { CurrentBranch = @ref }
                : new RepoState(true, @ref, [], []);
            return new CommandResult(true, msg, null, null, _currentState);
        }
        catch (JSException ex)
        {
            return new CommandResult(false, "", $"Git engine error: {ex.Message}", null, null);
        }
    }

    public async Task<CommandResult> MergeAsync(string branch)
    {
        try
        {
            var json = await _gitJs.GitMergeAsync(branch);
            var msg = json.GetProperty("message").GetString() ?? "Merge complete.";
            return new CommandResult(true, msg, null, null, _currentState);
        }
        catch (JSException ex)
        {
            return new CommandResult(false, "", $"Git engine error: {ex.Message}", null, null);
        }
    }

    public async Task<CommandResult> GetLogAsync(int depth = 20)
    {
        try
        {
            var json = await _gitJs.GitLogAsync(depth);
            var msg = json.GetProperty("message").GetString() ?? "(no commits yet)";
            return new CommandResult(true, msg, null, null, _currentState);
        }
        catch (JSException ex)
        {
            return new CommandResult(false, "", $"Git engine error: {ex.Message}", null, null);
        }
    }

    public Task ClearAsync()
    {
        _history.Clear();
        StateChanged?.Invoke();
        return Task.CompletedTask;
    }

    public async Task ResetAsync()
    {
        try
        {
            lock (_stateLock)
            {
                _history.Clear();
                _currentState = null;
            }
            
            // Clear localStorage
            await _sessionStorage.ClearAllAsync();
            
            // Reset IndexedDB via git interop
            await _gitJs.GitResetAsync();
            
            System.Diagnostics.Debug.WriteLine("[GitSimulatorService] Complete reset finished (history, state, storage, IndexedDB)");
            StateChanged?.Invoke();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[GitSimulatorService] Error during reset: {ex.Message}");
            // Errors are logged but don't rethrow — partial reset is acceptable
        }
    }

    private CommandResult BuildStatusResult()
    {
        if (_currentState is null)
            return new CommandResult(false, "", "Not a git repository. Run 'git init' first.", "git init", null);

        var staged = _currentState.StagedFiles.Count > 0
            ? $"Changes to be committed:\n{string.Join("\n", _currentState.StagedFiles.Select(f => $"  modified: {f}"))}\n"
            : "";
        var untracked = _currentState.UntrackedFiles.Count > 0
            ? $"Untracked files:\n{string.Join("\n", _currentState.UntrackedFiles.Select(f => $"  {f}"))}\n"
            : "";
        var clean = staged.Length == 0 && untracked.Length == 0
            ? "nothing to commit, working tree clean"
            : "";

        var output = $"On branch {_currentState.CurrentBranch ?? "main"}\n{staged}{untracked}{clean}".TrimEnd();
        return new CommandResult(true, output, null, null, _currentState);
    }

    private async Task TryUpdateGraphAsync()
    {
        lock (_stateLock)
        {
            if (_currentState?.IsInitialized != true) return;
        }
        
        try
        {
            var json = await _gitJs.GitGetGraphAsync();
            var graph = CommitGraph.FromJson(json);
            lock (_stateLock)
            {
                if (_currentState is not null)
                {
                    _currentState = _currentState with { Graph = graph };
                }
            }
        }
        catch (JsonException) { /* Graph is best-effort — malformed JSON */ }
        catch (InvalidOperationException) { /* Graph is best-effort — invalid state */ }
        catch (TimeoutException) { /* Graph is best-effort — JS interop timeout */ }
    }

    private const string HelpText =
        """
        Available git commands:
          git init              Initialize a new repository
          git add <file>        Stage a file (use '.' to stage all)
          git commit -m "msg"   Create a commit
          git branch <name>     Create a new branch
          git checkout <ref>    Switch to a branch
          git checkout -b <ref> Create and switch to a branch
          git merge <branch>    Merge a branch into current
          git log               Show commit history
          git status            Show working tree status
          git help              Show this help text
        """;
}
