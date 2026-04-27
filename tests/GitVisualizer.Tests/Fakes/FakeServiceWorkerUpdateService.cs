// tests/GitVisualizer.Tests/Fakes/FakeServiceWorkerUpdateService.cs
using GitVisualizer.Services;

namespace GitVisualizer.Tests.Fakes;

internal sealed class FakeServiceWorkerUpdateService : IServiceWorkerUpdateService
{
    public event EventHandler? UpdateDetected;

    public Task InitializeAsync() => Task.CompletedTask;

    public Task ReloadWithNewVersionAsync() => Task.CompletedTask;

    public void RaiseUpdateDetected() => UpdateDetected?.Invoke(this, EventArgs.Empty);
}
