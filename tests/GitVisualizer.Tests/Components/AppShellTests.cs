// tests/GitVisualizer.Tests/Components/AppShellTests.cs
using Bunit;
using GitVisualizer.Components.Layout;
using GitVisualizer.Components.Sandbox;
using GitVisualizer.Interop;
using GitVisualizer.Services;
using GitVisualizer.Tests.Fakes;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using MudBlazor.Services;

namespace GitVisualizer.Tests.Components;

/// <summary>Tests for CommitGraphPanel and SplitPane (no popover needed)</summary>
public class AppShellTests : BunitContext
{
    public AppShellTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddMudServices();
        Services.AddSingleton<IThemeService>(new FakeThemeService());
        Services.AddSingleton<ILocalStorageService>(new FakeLocalStorageService());
        Services.AddSingleton<SplitPaneJsInterop>();
    }

    [Fact]
    public void CommitGraphPanel_BeforeAnyCommands_ShowsEmptyStateHint()
    {
        var cut = Render<CommitGraphPanel>();
        Assert.Contains("Your repository will appear here", cut.Markup);
    }

    [Fact]
    public void CommitGraphPanel_WithCommands_DoesNotShowEmptyState()
    {
        var cut = Render<CommitGraphPanel>(p => p.Add(c => c.HasCommands, true));
        Assert.DoesNotContain("Your repository will appear here", cut.Markup);
        Assert.Contains("mermaid-graph", cut.Markup);
    }

    [Fact]
    public void SplitPane_Renders_LeftAndRightContent()
    {
        var cut = Render<SplitPane>(p =>
        {
            p.Add(c => c.LeftContent, "<p>L</p>");
            p.Add(c => c.RightContent, "<p>R</p>");
        });
        Assert.Contains("<p>L</p>", cut.Markup);
        Assert.Contains("<p>R</p>", cut.Markup);
    }
}

/// <summary>
/// Separate test class for MainLayout tests that render MudPopoverProvider.
/// Overrides Dispose to tolerate MudBlazor.PopoverService IAsyncDisposable-only teardown.
/// </summary>
public class MainLayoutToolbarTests : BunitContext
{
    public MainLayoutToolbarTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddMudServices();
        Services.AddSingleton<IThemeService>(new FakeThemeService());
        Services.AddSingleton<ILocalStorageService>(new FakeLocalStorageService());
    }

    protected override void Dispose(bool disposing)
    {
        try { base.Dispose(disposing); }
        catch (InvalidOperationException ex) when (ex.Message.Contains("IAsyncDisposable")) { }
        catch (AggregateException ex) when (ex.InnerExceptions.All(
            e => e is InvalidOperationException ioe && ioe.Message.Contains("IAsyncDisposable"))) { }
    }

    [Fact]
    public void MainLayout_Toolbar_ContainsFourActionItems()
    {
        RenderFragment emptyBody = _ => { };
        var cut = Render(b => {
            b.OpenComponent<MudBlazor.MudPopoverProvider>(0);
            b.CloseComponent();
            b.OpenComponent<MainLayout>(1);
            b.AddAttribute(2, "Body", emptyBody);
            b.CloseComponent();
        });
        var ariaLabeled = cut.FindAll("[aria-label]");
        Assert.True(ariaLabeled.Count >= 4,
            $"Expected at least 4 aria-label elements but found {ariaLabeled.Count}");
    }
}