// src/GitVisualizer/Interop/GitJsInterop.cs
using Microsoft.JSInterop;
using System.Text.Json;

namespace GitVisualizer.Interop;

/// <summary>
/// JS interop wrapper for git-interop.js. The only C# file that calls git-interop.js functions.
/// Module handle is imported once and cached; disposed via IAsyncDisposable.
/// </summary>
public sealed class GitJsInterop : IAsyncDisposable, IDisposable
{
    private readonly IJSRuntime _js;
    private IJSObjectReference? _module;

    public GitJsInterop(IJSRuntime js) => _js = js;

    private async ValueTask<IJSObjectReference> GetModuleAsync()
        => _module ??= await _js.InvokeAsync<IJSObjectReference>(
               "import", "./js/git-interop.js");

    public async ValueTask<JsonElement> GitInitAsync()
        => await (await GetModuleAsync()).InvokeAsync<JsonElement>("gitInit");

    public async ValueTask<JsonElement> GitAddAsync(string filepath = ".")
        => await (await GetModuleAsync()).InvokeAsync<JsonElement>("gitAdd", filepath);

    public async ValueTask<JsonElement> GitCommitAsync(string message)
        => await (await GetModuleAsync()).InvokeAsync<JsonElement>("gitCommit", message);

    public async ValueTask<JsonElement> GitBranchAsync(string name)
        => await (await GetModuleAsync()).InvokeAsync<JsonElement>("gitBranch", name);

    public async ValueTask<JsonElement> GitCheckoutAsync(string @ref, bool createBranch = false)
        => await (await GetModuleAsync()).InvokeAsync<JsonElement>("gitCheckout", @ref, createBranch);

    public async ValueTask<JsonElement> GitMergeAsync(string branch)
        => await (await GetModuleAsync()).InvokeAsync<JsonElement>("gitMerge", branch);

    public async ValueTask<JsonElement> GitLogAsync(int depth = 20)
        => await (await GetModuleAsync()).InvokeAsync<JsonElement>("gitLog", depth);

    public async ValueTask<JsonElement> GitGetGraphAsync()
        => await (await GetModuleAsync()).InvokeAsync<JsonElement>("gitGetGraph");

    /// <summary>Reset git repository state by wiping lightning-fs IndexedDB namespace.</summary>
    public async ValueTask GitResetAsync()
    {
        try
        {
            await (await GetModuleAsync()).InvokeVoidAsync("gitReset");
            System.Diagnostics.Debug.WriteLine("[GitJsInterop] Git reset completed (IndexedDB wiped)");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[GitJsInterop] Error during git reset: {ex.Message}");
            // Don't rethrow — allow reset to continue
        }
    }

    public void Dispose()
    {
        // IAsyncDisposable only — sync Dispose() is a no-op to avoid WASM deadlock
    }

    public async ValueTask DisposeAsync()
    {
        if (_module is not null)
        {
            await _module.DisposeAsync();
            _module = null;
        }
    }
}
