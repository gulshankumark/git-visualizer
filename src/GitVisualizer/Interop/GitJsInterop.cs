// src/GitVisualizer/Interop/GitJsInterop.cs
using Microsoft.JSInterop;

namespace GitVisualizer.Interop;

/// <summary>
/// JS interop wrapper for git-interop.js.
/// Full git operations are wired in Story 2.3; this provides the interop scaffold.
/// </summary>
public sealed class GitJsInterop : IAsyncDisposable, IDisposable
{
    private readonly IJSRuntime _js;
    private IJSObjectReference? _module;

    public GitJsInterop(IJSRuntime js) => _js = js;

    private async ValueTask<IJSObjectReference> GetModuleAsync()
        => _module ??= await _js.InvokeAsync<IJSObjectReference>(
               "import", "./js/git-interop.js");

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
