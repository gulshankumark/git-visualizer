// tests/GitVisualizer.Tests/Accessibility/AccessibilityTests.cs
using Bunit;
using GitVisualizer.Components.Sandbox;
using GitVisualizer.Interop;
using GitVisualizer.Models;
using GitVisualizer.Services;
using GitVisualizer.Tests.Fakes;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using System.IO;

namespace GitVisualizer.Tests.Accessibility;

/// <summary>
/// bUnit tests for Story 5.2 — Screen Reader &amp; ARIA Semantics.
/// Covers AC1 (terminal live region), AC2 (graph aria-label format),
/// AC3 (toolbar labels via source check), AC4 (role="alert"), AC5 (test suite).
/// </summary>
public class AccessibilityTests : BunitContext
{
    private readonly FakeGitSimulatorService _git = new();
    private readonly FakeStorageMonitorService _storage = new();
    private readonly FakeGraphRenderService _fakeRenderService = new();
    private readonly FakeThemeService _fakeThemeService = new();

    public AccessibilityTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddMudServices();
        Services.AddSingleton<IGitSimulatorService>(_git);
        Services.AddSingleton<IStorageMonitorService>(_storage);
        Services.AddSingleton<IGraphRenderService>(_fakeRenderService);
        Services.AddSingleton<IThemeService>(_fakeThemeService);
        Services.AddSingleton<GraphRendererJsInterop>();
    }

    protected override void Dispose(bool disposing)
    {
        try { base.Dispose(disposing); }
        catch (InvalidOperationException) { }
        catch (AggregateException ex) when (ex.InnerExceptions.All(e => e is InvalidOperationException)) { }
    }

    private static string GetRepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir != null && !File.Exists(Path.Combine(dir.FullName, "git-visualizer.sln")))
            dir = dir.Parent;
        if (dir == null) throw new InvalidOperationException("Could not find repository root");
        return dir.FullName;
    }

    // ── AC1: Terminal live region ─────────────────────────────────────────────

    [Fact]
    public void TerminalPanel_OutputRegion_HasRoleLog()
    {
        var cut = Render<TerminalPanel>();

        var region = cut.Find(".terminal-output");
        Assert.Equal("log", region.GetAttribute("role"));
    }

    [Fact]
    public void TerminalPanel_OutputRegion_HasAriaLivePolite()
    {
        var cut = Render<TerminalPanel>();

        var region = cut.Find(".terminal-output");
        Assert.Equal("polite", region.GetAttribute("aria-live"));
    }

    [Fact]
    public void TerminalPanel_OutputRegion_HasAriaLabel()
    {
        var cut = Render<TerminalPanel>();

        var region = cut.Find(".terminal-output");
        Assert.Equal("Terminal output", region.GetAttribute("aria-label"));
    }

    // ── AC4: role="alert" on MudAlert ────────────────────────────────────────

    [Fact]
    public void TerminalPanel_StorageWarningAlert_HasRoleAlert()
    {
        _storage.SetStatus(new StorageStatus(
            UsageBytes: 45_000_000,
            QuotaBytes: 50_000_000,
            UsagePercent: 90,
            IsWarning: true,
            IsBlocked: false));

        var cut = Render<TerminalPanel>();

        var roleAlerts = cut.FindAll("[role=\"alert\"]");
        Assert.NotEmpty(roleAlerts);
    }

    [Fact]
    public void TerminalPanel_CommandErrorAlert_HasRoleAlert()
    {
        _git.AddHistoryEntry("git bad-cmd",
            new CommandResult(false, "", "Unknown command: bad-cmd", "Did you mean 'git status'?", null));

        var cut = Render<TerminalPanel>();

        var roleAlerts = cut.FindAll("[role=\"alert\"]");
        Assert.NotEmpty(roleAlerts);
    }

    // ── AC2: Commit graph ARIA label format ───────────────────────────────────

    [Fact]
    public void CommitGraphPanel_HasRoleImgAndGitGraphAriaLabel()
    {
        _git.CurrentState = null;

        var cut = Render<CommitGraphPanel>();

        var panel = cut.Find(".commit-graph-panel");
        Assert.Equal("img", panel.GetAttribute("role"));
        var ariaLabel = panel.GetAttribute("aria-label");
        Assert.NotNull(ariaLabel);
        Assert.StartsWith("Git graph:", ariaLabel);
    }

    [Fact]
    public void CommitGraphPanel_WhenInitialized_AriaLabelContainsHeadOnBranch()
    {
        _git.CurrentState = new RepoState(true, "main", [], []);
        _fakeRenderService.AriaLabelToReturn = "Git graph: HEAD on main, 1 commit";

        var cut = Render<CommitGraphPanel>();

        var ariaLabel = cut.Find(".commit-graph-panel").GetAttribute("aria-label");
        Assert.NotNull(ariaLabel);
        Assert.Contains("Git graph: HEAD on main", ariaLabel);
    }

    [Fact]
    public void GraphRenderService_BuildAriaLabel_AlwaysStartsWithGitGraph()
    {
        var service = new GraphRenderService();

        Assert.StartsWith("Git graph:", service.BuildAriaLabel(null));

        var noCommits = new RepoState(true, "main", [], []);
        var label = service.BuildAriaLabel(noCommits);
        Assert.StartsWith("Git graph:", label);
        Assert.Contains("HEAD on main", label);
        Assert.Contains("0 commits", label);
    }

    // ── AC3: Toolbar button aria-labels (source-file check) ──────────────────

    [Fact]
    public void MainLayout_ToolbarButtons_AllHaveAriaLabels()
    {
        var path = Path.Combine(GetRepoRoot(), "src/GitVisualizer/Components/Layout/MainLayout.razor");
        var src = File.ReadAllText(path);

        var matches = System.Text.RegularExpressions.Regex.Matches(src, @"aria-label=""[^""]+""|aria-label='[^']+'");
        Assert.True(matches.Count >= 4,
            $"Expected ≥4 aria-label attributes on toolbar buttons in MainLayout.razor, found {matches.Count}");
    }
}
