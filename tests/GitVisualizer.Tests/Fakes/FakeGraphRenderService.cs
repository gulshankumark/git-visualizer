// tests/GitVisualizer.Tests/Fakes/FakeGraphRenderService.cs
using GitVisualizer.Models;
using GitVisualizer.Services;

namespace GitVisualizer.Tests.Fakes;

internal sealed class FakeGraphRenderService : IGraphRenderService
{
    public GraphRenderPayload? PayloadToReturn { get; set; }
    public string AriaLabelToReturn { get; set; } = "Git graph: HEAD on main, 1 commit";

    public GraphRenderPayload? BuildPayload(RepoState state) => PayloadToReturn;
    public string BuildAriaLabel(RepoState? state) => AriaLabelToReturn;
}
