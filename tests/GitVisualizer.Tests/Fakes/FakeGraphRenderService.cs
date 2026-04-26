// tests/GitVisualizer.Tests/Fakes/FakeGraphRenderService.cs
using GitVisualizer.Models;
using GitVisualizer.Services;

namespace GitVisualizer.Tests.Fakes;

internal sealed class FakeGraphRenderService : IGraphRenderService
{
    public string SyntaxToReturn { get; set; } = "";
    public string AriaLabelToReturn { get; set; } = "Commit graph: main branch, 1 commit.";

    public string ToMermaidSyntax(RepoState state) => SyntaxToReturn;
    public string BuildAriaLabel(RepoState? state) => AriaLabelToReturn;
}
