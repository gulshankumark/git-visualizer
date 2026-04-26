using GitVisualizer.Models;
using GitVisualizer.Services;

namespace GitVisualizer.Tests.Fakes;

/// <summary>Fake implementation of IStorageMonitorService for testing.</summary>
public class FakeStorageMonitorService : IStorageMonitorService
{
    private StorageStatus? _status = new(
        UsageBytes: 0,
        QuotaBytes: 50_000_000,
        UsagePercent: 0,
        IsWarning: false,
        IsBlocked: false
    );

    public event Action<StorageStatus>? OnStorageStatusChanged;

    public Task<long> GetUsageBytesAsync()
        => Task.FromResult(_status?.UsageBytes ?? 0);

    public Task<long> GetQuotaBytesAsync()
        => Task.FromResult(_status?.QuotaBytes ?? 50_000_000);

    public Task<bool> IsWarningThresholdCrossedAsync()
        => Task.FromResult(_status?.IsWarning ?? false);

    public Task<bool> IsBlockThresholdCrossedAsync()
        => Task.FromResult(_status?.IsBlocked ?? false);

    public Task<StorageStatus> GetStatusAsync()
        => Task.FromResult(_status ?? new(0, 50_000_000, 0, false, false));

    /// <summary>Set the fake storage status for testing purposes.</summary>
    public void SetStatus(StorageStatus status)
    {
        _status = status;
        OnStorageStatusChanged?.Invoke(status);
    }
}
