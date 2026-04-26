// tests/GitVisualizer.Tests/Components/CommitGraphPanelTests.cs
using Bunit;
using GitVisualizer.Components.Sandbox;
using GitVisualizer.Interop;
using GitVisualizer.Models;
using GitVisualizer.Services;
using GitVisualizer.Tests.Fakes;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;

namespace GitVisualizer.Tests.Components;

public class CommitGraphPanelTests : BunitContext
{
    private readonly FakeGitSimulatorService _fakeGitService = new();
    private readonly FakeGraphRenderService _fakeRenderService = new();
    private readonly FakeThemeService _fakeThemeService = new();

    public CommitGraphPanelTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddMudServices();
        Services.AddSingleton<IGitSimulatorService>(_fakeGitService);
        Services.AddSingleton<IGraphRenderService>(_fakeRenderService);
        Services.AddSingleton<IThemeService>(_fakeThemeService);
        Services.AddSingleton<GraphRendererJsInterop>();
    }

    [Fact]
    public void CommitGraphPanel_WhenNotInitialized_ShowsEmptyState()
    {
        _fakeGitService.CurrentState = null;
        var cut = Render<CommitGraphPanel>();
        Assert.Contains("git init", cut.Markup);
        Assert.DoesNotContain("git-graph", cut.Markup);
    }

    [Fact]
    public void CommitGraphPanel_WhenInitialized_ShowsGitGraphContainer()
    {
        _fakeGitService.CurrentState = new RepoState(true, "main", [], []);
        var cut = Render<CommitGraphPanel>();
        Assert.Contains("git-graph", cut.Markup);
        Assert.DoesNotContain("git init", cut.Markup);
    }

    [Fact]
    public void CommitGraphPanel_WhenNotInitialized_HasDefaultAriaLabel()
    {
        _fakeGitService.CurrentState = null;
        _fakeRenderService.AriaLabelToReturn = "Commit graph: no repository initialised.";
        var cut = Render<CommitGraphPanel>();
        Assert.Contains("Commit graph:", cut.Markup);
    }

    [Fact]
    public void CommitGraphPanel_WhenInitialized_HasCustomAriaLabel()
    {
        _fakeGitService.CurrentState = new RepoState(true, "main", [], []);
        _fakeRenderService.AriaLabelToReturn = "Commit graph: main branch, 2 commits.";
        var cut = Render<CommitGraphPanel>();
        Assert.Contains("Commit graph: main branch, 2 commits.", cut.Markup);
    }

    [Fact]
    public async Task CommitGraphPanel_AfterStateChanged_UpdatesDisplay()
    {
        _fakeGitService.CurrentState = null;
        var cut = Render<CommitGraphPanel>();
        Assert.Contains("git init", cut.Markup);

        // Simulate state change: repo now initialized
        _fakeGitService.CurrentState = new RepoState(true, "main", [], []);
        _fakeGitService.RaiseStateChanged();

        // Allow async debounce + InvokeAsync to propagate
        await Task.Delay(200);
        cut.Render(); // force re-render check
        Assert.Contains("git-graph", cut.Markup);
    }

    [Fact]
    public void CommitGraphPanel_HasTabIndex_ForKeyboardAccess()
    {
        _fakeGitService.CurrentState = null;
        var cut = Render<CommitGraphPanel>();
        Assert.Contains("tabindex=\"0\"", cut.Markup);
    }
}
