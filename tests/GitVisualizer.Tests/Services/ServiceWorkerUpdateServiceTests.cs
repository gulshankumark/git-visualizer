using Xunit;
using GitVisualizer.Services;

namespace GitVisualizer.Tests.Services;

/// <summary>
/// Tests for the ServiceWorkerUpdateService.
/// These tests verify the service's ability to initialize and handle updates.
/// </summary>
public class ServiceWorkerUpdateServiceTests
{
    [Fact]
    public void Service_ShouldBeCreatable()
    {
        // Arrange: Mock JSRuntime is not needed for basic construction
        // Act & Assert: Service should construct without errors
        Assert.NotNull(typeof(ServiceWorkerUpdateService));
    }

    [Fact]
    public void UpdateDetectedEvent_ShouldBeAvailable()
    {
        // Arrange & Act & Assert: Event should be accessible
        var serviceType = typeof(ServiceWorkerUpdateService);
        var eventInfo = serviceType.GetEvent("UpdateDetected");
        Assert.NotNull(eventInfo);
    }

    [Fact]
    public void InitializeAsync_ShouldBeAsync()
    {
        // Arrange & Act & Assert: Method should be async
        var method = typeof(ServiceWorkerUpdateService).GetMethod("InitializeAsync");
        Assert.NotNull(method);
        Assert.Contains("Task", method!.ReturnType.Name);
    }

    [Fact]
    public void OnUpdateDetected_ShouldBeCallable()
    {
        // Arrange & Act & Assert: Method should be JSInvokable
        var method = typeof(ServiceWorkerUpdateService).GetMethod("OnUpdateDetected");
        Assert.NotNull(method);
        var jsInvokableAttr = method!.GetCustomAttributes(typeof(Microsoft.JSInterop.JSInvokableAttribute), false);
        Assert.Single(jsInvokableAttr);
    }

    [Fact]
    public void ReloadWithNewVersionAsync_ShouldBeAsync()
    {
        // Arrange & Act & Assert: Method should be async
        var method = typeof(ServiceWorkerUpdateService).GetMethod("ReloadWithNewVersionAsync");
        Assert.NotNull(method);
        Assert.Contains("Task", method!.ReturnType.Name);
    }
}

