using Microsoft.JSInterop;

namespace GitVisualizer.Services;

/// <summary>
/// Service for managing PWA service worker updates and notifications.
/// Detects new versions and notifies the user to reload the app.
/// </summary>
public interface IServiceWorkerUpdateService
{
    /// <summary>
    /// Event fired when a service worker update is detected.
    /// </summary>
    event EventHandler? UpdateDetected;

    /// <summary>
    /// Initialize the service worker update detection.
    /// Must be called once from the main layout during initialization.
    /// </summary>
    Task InitializeAsync();

    /// <summary>
    /// Tell the service worker to activate the new version and reload the page.
    /// </summary>
    Task ReloadWithNewVersionAsync();
}

/// <summary>
/// Implementation of IServiceWorkerUpdateService.
/// Manages communication between the Blazor app and the service worker.
/// </summary>
public class ServiceWorkerUpdateService : IServiceWorkerUpdateService
{
    private readonly IJSRuntime _jsRuntime;
    private bool _isInitialized;

    public event EventHandler? UpdateDetected;

    public ServiceWorkerUpdateService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime ?? throw new ArgumentNullException(nameof(jsRuntime));
    }

    public async Task InitializeAsync()
    {
        if (_isInitialized)
            return;

        _isInitialized = true;

        try
        {
            // Initialize the update detection mechanism
            await _jsRuntime.InvokeVoidAsync("initServiceWorkerUpdateDetection",
                DotNetObjectReference.Create(this));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to initialize service worker update detection: {ex.Message}");
        }
    }

    [JSInvokable]
    public async Task OnUpdateDetected()
    {
        System.Diagnostics.Debug.WriteLine("[ServiceWorkerUpdateService] Update detected");
        UpdateDetected?.Invoke(this, EventArgs.Empty);
        await Task.CompletedTask;
    }

    public async Task ReloadWithNewVersionAsync()
    {
        try
        {
            await _jsRuntime.InvokeVoidAsync("reloadWithNewServiceWorkerVersion");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to reload with new service worker version: {ex.Message}");
            // Fallback: force a page reload
            await _jsRuntime.InvokeVoidAsync("location.reload");
        }
    }
}
