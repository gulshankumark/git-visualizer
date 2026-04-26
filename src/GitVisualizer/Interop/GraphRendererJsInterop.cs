// src/GitVisualizer/Interop/GraphRendererJsInterop.cs
using GitVisualizer.Models;
using Microsoft.JSInterop;

namespace GitVisualizer.Interop;

/// <summary>
/// JS interop wrapper for gitgraph-renderer.js. Module handle is imported once and cached.
/// The renderer engine (Stage 1: @gitgraph/js, Stage 2: custom SVG) is hidden behind this boundary —
/// the C# side ships a structured <see cref="GraphRenderPayload"/>, never a DSL string.
/// </summary>
public sealed class GraphRendererJsInterop : IAsyncDisposable, IDisposable
{
    private readonly IJSRuntime _js;
    private IJSObjectReference? _module;

    public GraphRendererJsInterop(IJSRuntime js) => _js = js;

    public async Task InitAsync(bool isDark)
    {
        var module = await GetModuleAsync();
        await module.InvokeVoidAsync("initRenderer", isDark);
    }

    public async Task RenderAsync(string containerId, GraphRenderPayload payload)
    {
        var module = await GetModuleAsync();
        await module.InvokeVoidAsync("renderGraph", containerId, payload);
    }

    public async Task ScrollToHeadAsync(string containerId)
    {
        var module = await GetModuleAsync();
        await module.InvokeVoidAsync("scrollToHead", containerId);
    }

    private async ValueTask<IJSObjectReference> GetModuleAsync()
        => _module ??= await _js.InvokeAsync<IJSObjectReference>(
               "import", "./js/gitgraph-renderer.js");

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
