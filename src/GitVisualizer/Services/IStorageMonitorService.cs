using GitVisualizer.Models;

namespace GitVisualizer.Services;

/// <summary>
/// Monitors IndexedDB storage usage and provides threshold warnings for app stability.
/// </summary>
public interface IStorageMonitorService
{
    /// <summary>
    /// Gets the current IndexedDB usage in bytes.
    /// </summary>
    Task<long> GetUsageBytesAsync();

    /// <summary>
    /// Gets the IndexedDB quota limit in bytes.
    /// </summary>
    Task<long> GetQuotaBytesAsync();

    /// <summary>
    /// Checks if usage has crossed the 45MB warning threshold.
    /// </summary>
    Task<bool> IsWarningThresholdCrossedAsync();

    /// <summary>
    /// Checks if usage has crossed the 50MB block threshold.
    /// </summary>
    Task<bool> IsBlockThresholdCrossedAsync();

    /// <summary>
    /// Gets current storage status.
    /// </summary>
    Task<StorageStatus> GetStatusAsync();

    /// <summary>
    /// Event fired when storage status changes (thresholds crossed or cleared).
    /// Fired only on threshold transition, not on every query.
    /// </summary>
    event Action<StorageStatus>? OnStorageStatusChanged;
}
