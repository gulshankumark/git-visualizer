// src/GitVisualizer/Interop/MermaidJsInterop.cs
using Microsoft.JSInterop;

namespace GitVisualizer.Interop;

/// <summary>
/// JS interop wrapper for mermaid-interop.js.
/// Module handle is imported once and cached; disposed via IAsyncDisposable.
/// </summary>
public sealed class MermaidJsInterop : IAsyncDisposable, IDisposable
{
    private readonly IJSRuntime _js;
    private IJSObjectReference? _module;

    public MermaidJsInterop(IJSRuntime js) => _js = js;

    public async Task InitAsync(bool isDark)
    {
        var module = await GetModuleAsync();
        await module.InvokeVoidAsync("initMermaid", isDark);
    }

    public async Task RenderGraphAsync(string containerId, string syntax)
    {
        var module = await GetModuleAsync();
        await module.InvokeVoidAsync("renderGraph", containerId, syntax);
    }

    public async Task ScrollToHeadAsync(string containerId)
    {
        var module = await GetModuleAsync();
        await module.InvokeVoidAsync("scrollToHead", containerId);
    }

    private async ValueTask<IJSObjectReference> GetModuleAsync()
        => _module ??= await _js.InvokeAsync<IJSObjectReference>(
               "import", "./js/mermaid-interop.js");

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
