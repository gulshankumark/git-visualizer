using GitVisualizer.Interop;
using GitVisualizer.Models;

namespace GitVisualizer.Services;

/// <summary>
/// Monitors IndexedDB storage usage and fires events when thresholds are crossed.
/// </summary>
public sealed class StorageMonitorService : IStorageMonitorService
{
    private readonly IStorageJsInterop _storage;
    private StorageStatus? _previousStatus;

    // Thresholds in bytes
    private const long WARN_THRESHOLD_BYTES = 45_000_000;     // 45MB
    private const long BLOCK_THRESHOLD_BYTES = 50_000_000;    // 50MB

    public event Action<StorageStatus>? OnStorageStatusChanged;

    public StorageMonitorService(IStorageJsInterop storage)
    {
        _storage = storage;
    }

    public async Task<long> GetUsageBytesAsync()
    {
        var estimate = await _storage.GetStorageEstimateAsync();
        return estimate.Usage;
    }

    public async Task<long> GetQuotaBytesAsync()
    {
        var estimate = await _storage.GetStorageEstimateAsync();
        return estimate.Quota;
    }

    public async Task<bool> IsWarningThresholdCrossedAsync()
    {
        var usage = await GetUsageBytesAsync();
        return usage >= WARN_THRESHOLD_BYTES;
    }

    public async Task<bool> IsBlockThresholdCrossedAsync()
    {
        var usage = await GetUsageBytesAsync();
        return usage >= BLOCK_THRESHOLD_BYTES;
    }

    public async Task<StorageStatus> GetStatusAsync()
    {
        var estimate = await _storage.GetStorageEstimateAsync();
        var usagePercent = estimate.Quota > 0 ? (estimate.Usage * 100.0) / estimate.Quota : 0;
        var isWarning = estimate.Usage >= WARN_THRESHOLD_BYTES;
        var isBlocked = estimate.Usage >= BLOCK_THRESHOLD_BYTES;

        var status = new StorageStatus(
            UsageBytes: estimate.Usage,
            QuotaBytes: estimate.Quota,
            UsagePercent: usagePercent,
            IsWarning: isWarning,
            IsBlocked: isBlocked
        );

        // Fire event only if status changed
        FireThresholdChangedEvent(status);

        return status;
    }

    private void FireThresholdChangedEvent(StorageStatus newStatus)
    {
        // No previous status or thresholds changed
        if (_previousStatus == null ||
            _previousStatus.IsWarning != newStatus.IsWarning ||
            _previousStatus.IsBlocked != newStatus.IsBlocked)
        {
            _previousStatus = newStatus;
            OnStorageStatusChanged?.Invoke(newStatus);
        }
    }
}
