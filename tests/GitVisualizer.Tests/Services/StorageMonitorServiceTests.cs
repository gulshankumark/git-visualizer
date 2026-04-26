using GitVisualizer.Interop;
using GitVisualizer.Models;
using GitVisualizer.Services;
using Xunit;

namespace GitVisualizer.Tests.Services;

public class StorageMonitorServiceTests
{
    private class FakeStorageJsInterop : IStorageJsInterop
    {
        private StorageEstimate _estimate = new(0, 50_000_000);

        public void SetEstimate(long usage, long quota) => _estimate = new(usage, quota);
        public Task<StorageEstimate> GetStorageEstimateAsync() => Task.FromResult(_estimate);
    }

    private class TestableStorageMonitorService
    {
        private readonly FakeStorageJsInterop _fakeStorage = new();

        public StorageMonitorService CreateService() => new(_fakeStorage);
        public FakeStorageJsInterop FakeStorage => _fakeStorage;
    }

    [Fact]
    public async Task GetUsageBytesAsync_ReturnsCurrentUsage()
    {
        // Arrange
        var harness = new TestableStorageMonitorService();
        harness.FakeStorage.SetEstimate(25_000_000, 50_000_000);
        var service = harness.CreateService();

        // Act
        var usage = await service.GetUsageBytesAsync();

        // Assert
        Assert.Equal(25_000_000, usage);
    }

    [Fact]
    public async Task GetQuotaBytesAsync_ReturnsQuotaLimit()
    {
        // Arrange
        var harness = new TestableStorageMonitorService();
        harness.FakeStorage.SetEstimate(25_000_000, 50_000_000);
        var service = harness.CreateService();

        // Act
        var quota = await service.GetQuotaBytesAsync();

        // Assert
        Assert.Equal(50_000_000, quota);
    }

    [Fact]
    public async Task IsWarningThresholdCrossedAsync_ReturnsTrueWhenUsageAbove45MB()
    {
        // Arrange
        var harness = new TestableStorageMonitorService();
        harness.FakeStorage.SetEstimate(46_000_000, 50_000_000); // 46MB > 45MB threshold
        var service = harness.CreateService();

        // Act
        var isCrossed = await service.IsWarningThresholdCrossedAsync();

        // Assert
        Assert.True(isCrossed);
    }

    [Fact]
    public async Task IsWarningThresholdCrossedAsync_ReturnsFalseWhenUsageBelow45MB()
    {
        // Arrange
        var harness = new TestableStorageMonitorService();
        harness.FakeStorage.SetEstimate(40_000_000, 50_000_000); // 40MB < 45MB threshold
        var service = harness.CreateService();

        // Act
        var isCrossed = await service.IsWarningThresholdCrossedAsync();

        // Assert
        Assert.False(isCrossed);
    }

    [Fact]
    public async Task IsWarningThresholdCrossedAsync_ReturnsTrueAtExactThreshold()
    {
        // Arrange
        var harness = new TestableStorageMonitorService();
        harness.FakeStorage.SetEstimate(45_000_000, 50_000_000); // Exactly 45MB
        var service = harness.CreateService();

        // Act
        var isCrossed = await service.IsWarningThresholdCrossedAsync();

        // Assert
        Assert.True(isCrossed);
    }

    [Fact]
    public async Task IsBlockThresholdCrossedAsync_ReturnsTrueWhenUsageAbove50MB()
    {
        // Arrange
        var harness = new TestableStorageMonitorService();
        harness.FakeStorage.SetEstimate(51_000_000, 50_000_000); // 51MB > 50MB threshold
        var service = harness.CreateService();

        // Act
        var isBlocked = await service.IsBlockThresholdCrossedAsync();

        // Assert
        Assert.True(isBlocked);
    }

    [Fact]
    public async Task IsBlockThresholdCrossedAsync_ReturnsFalseWhenUsageBelow50MB()
    {
        // Arrange
        var harness = new TestableStorageMonitorService();
        harness.FakeStorage.SetEstimate(45_000_000, 50_000_000); // 45MB < 50MB threshold
        var service = harness.CreateService();

        // Act
        var isBlocked = await service.IsBlockThresholdCrossedAsync();

        // Assert
        Assert.False(isBlocked);
    }

    [Fact]
    public async Task IsBlockThresholdCrossedAsync_ReturnsTrueAtExactThreshold()
    {
        // Arrange
        var harness = new TestableStorageMonitorService();
        harness.FakeStorage.SetEstimate(50_000_000, 50_000_000); // Exactly 50MB
        var service = harness.CreateService();

        // Act
        var isBlocked = await service.IsBlockThresholdCrossedAsync();

        // Assert
        Assert.True(isBlocked);
    }

    [Fact]
    public async Task GetStatusAsync_ReturnsCorrectStatus()
    {
        // Arrange
        var harness = new TestableStorageMonitorService();
        harness.FakeStorage.SetEstimate(30_000_000, 50_000_000); // 30MB = 60%
        var service = harness.CreateService();

        // Act
        var status = await service.GetStatusAsync();

        // Assert
        Assert.Equal(30_000_000, status.UsageBytes);
        Assert.Equal(50_000_000, status.QuotaBytes);
        Assert.Equal(60.0, status.UsagePercent);
        Assert.False(status.IsWarning); // 30MB < 45MB
        Assert.False(status.IsBlocked); // 30MB < 50MB
    }

    [Fact]
    public async Task GetStatusAsync_MarksWarningWhenAbove45MB()
    {
        // Arrange
        var harness = new TestableStorageMonitorService();
        harness.FakeStorage.SetEstimate(47_000_000, 50_000_000); // 47MB > 45MB
        var service = harness.CreateService();

        // Act
        var status = await service.GetStatusAsync();

        // Assert
        Assert.True(status.IsWarning);
        Assert.False(status.IsBlocked);
    }

    [Fact]
    public async Task GetStatusAsync_MarksBlockedWhenAbove50MB()
    {
        // Arrange
        var harness = new TestableStorageMonitorService();
        harness.FakeStorage.SetEstimate(51_000_000, 50_000_000); // 51MB > 50MB
        var service = harness.CreateService();

        // Act
        var status = await service.GetStatusAsync();

        // Assert
        Assert.True(status.IsWarning);  // Also warning since > 45MB
        Assert.True(status.IsBlocked);
    }

    [Fact]
    public async Task OnStorageStatusChanged_FiresOnThresholdCrossing()
    {
        // Arrange
        var harness = new TestableStorageMonitorService();
        harness.FakeStorage.SetEstimate(30_000_000, 50_000_000); // Below warning threshold
        var service = harness.CreateService();

        StorageStatus? firedStatus = null;
        service.OnStorageStatusChanged += status => firedStatus = status;

        // Act — first call should fire (no previous status)
        var status1 = await service.GetStatusAsync();

        // Assert — event fired on first call
        Assert.NotNull(firedStatus);
        Assert.False(firedStatus.IsWarning);

        // Act — second call below threshold should NOT fire
        firedStatus = null;
        var status2 = await service.GetStatusAsync();

        // Assert — event did not fire
        Assert.Null(firedStatus);

        // Act — cross warning threshold should fire
        harness.FakeStorage.SetEstimate(46_000_000, 50_000_000);
        var status3 = await service.GetStatusAsync();

        // Assert — event fired when threshold crossed
        Assert.NotNull(firedStatus);
        Assert.True(firedStatus.IsWarning);
    }

    [Fact]
    public async Task OnStorageStatusChanged_FiresOnThresholdClearing()
    {
        // Arrange
        var harness = new TestableStorageMonitorService();
        harness.FakeStorage.SetEstimate(46_000_000, 50_000_000); // Above warning threshold
        var service = harness.CreateService();

        StorageStatus? firedStatus = null;
        service.OnStorageStatusChanged += status => firedStatus = status;

        // Act — first call sets warning
        var status1 = await service.GetStatusAsync();

        // Assert
        Assert.NotNull(firedStatus);
        Assert.True(firedStatus.IsWarning);

        // Act — drop below threshold should fire
        harness.FakeStorage.SetEstimate(40_000_000, 50_000_000);
        firedStatus = null;
        var status2 = await service.GetStatusAsync();

        // Assert — event fired when threshold cleared
        Assert.NotNull(firedStatus);
        Assert.False(firedStatus.IsWarning);
    }

    [Fact]
    public async Task UsagePercentCalculation_IsAccurate()
    {
        // Arrange
        var harness = new TestableStorageMonitorService();
        harness.FakeStorage.SetEstimate(25_000_000, 50_000_000); // 50%
        var service = harness.CreateService();

        // Act
        var status = await service.GetStatusAsync();

        // Assert
        Assert.Equal(50.0, status.UsagePercent);
    }
}
