// src/GitVisualizer/Interop/ViewportJsInterop.cs
using Microsoft.JSInterop;

namespace GitVisualizer.Interop;

/// <summary>
/// JS interop wrapper for viewport-interop.js.
/// Provides mobile breakpoint detection (&lt;768px) and resize event subscription.
/// Same IJSObjectReference module pattern as SplitPaneJsInterop.
/// </summary>
public sealed class ViewportJsInterop : IAsyncDisposable, IDisposable
{
    private readonly IJSRuntime _js;
    private IJSObjectReference? _module;

    public ViewportJsInterop(IJSRuntime js) => _js = js;

    /// <summary>Returns true when viewport width is below 768px (mobile breakpoint).</summary>
    public async Task<bool> IsMobileAsync()
    {
        var module = await GetModuleAsync();
        return await module.InvokeAsync<bool>("isMobile");
    }

    /// <summary>
    /// Subscribes a .NET component to receive breakpoint-change callbacks.
    /// The target object must have a [JSInvokable] OnBreakpointChanged(bool isMobile) method.
    /// </summary>
    public async Task SubscribeAsync<T>(DotNetObjectReference<T> dotnetRef) where T : class
    {
        var module = await GetModuleAsync();
        await module.InvokeVoidAsync("subscribeResize", dotnetRef);
    }

    /// <summary>Removes the resize listener registered by SubscribeAsync.</summary>
    public async Task UnsubscribeAsync()
    {
        if (_module is null) return;
        await _module.InvokeVoidAsync("unsubscribeResize");
    }

    private async ValueTask<IJSObjectReference> GetModuleAsync()
        => _module ??= await _js.InvokeAsync<IJSObjectReference>(
               "import", "./js/viewport-interop.js");

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
