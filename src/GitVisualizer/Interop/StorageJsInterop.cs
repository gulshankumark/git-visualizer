using Microsoft.JSInterop;

namespace GitVisualizer.Interop;

/// <summary>
/// JS interop wrapper for storage-monitor-interop.js. Module handle is imported once and cached.
/// Provides access to navigator.storage.estimate() for IndexedDB quota monitoring.
/// </summary>
public sealed class StorageJsInterop : IAsyncDisposable, IDisposable, IStorageJsInterop
{
    private readonly IJSRuntime _js;
    private IJSObjectReference? _module;

    public StorageJsInterop(IJSRuntime js) => _js = js;

    /// <summary>
    /// Gets the current storage usage and quota estimates from navigator.storage.estimate().
    /// </summary>
    /// <returns>Object with Usage (bytes) and Quota (bytes) properties</returns>
    public async Task<StorageEstimate> GetStorageEstimateAsync()
    {
        var module = await GetModuleAsync();
        return await module.InvokeAsync<StorageEstimate>("getStorageEstimate");
    }

    private async ValueTask<IJSObjectReference> GetModuleAsync()
        => _module ??= await _js.InvokeAsync<IJSObjectReference>(
            "import", "./js/storage-monitor-interop.js");

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

/// <summary>
/// Result from navigator.storage.estimate().
/// </summary>
public record StorageEstimate(
    long Usage,    // bytes
    long Quota     // bytes
);
