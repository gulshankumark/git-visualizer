// src/GitVisualizer/Interop/SplitPaneJsInterop.cs
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace GitVisualizer.Interop;

public sealed class SplitPaneJsInterop : IAsyncDisposable, IDisposable
{
    private readonly IJSRuntime _js;
    private IJSObjectReference? _module;

    public SplitPaneJsInterop(IJSRuntime js) => _js = js;

    private async ValueTask<IJSObjectReference> GetModuleAsync()
        => _module ??= await _js.InvokeAsync<IJSObjectReference>(
               "import", "./js/split-pane-interop.js");

    public async ValueTask<double> GetContainerWidthAsync(ElementReference element)
    {
        var module = await GetModuleAsync();
        return await module.InvokeAsync<double>("getContainerWidth", element);
    }

    public void Dispose()
    {
        // IAsyncDisposable only — sync Dispose() is a no-op to avoid WASM deadlock
        // (single-threaded runtime cannot block on async disposal)
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
