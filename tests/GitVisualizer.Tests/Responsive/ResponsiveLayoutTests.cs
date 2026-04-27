// tests/GitVisualizer.Tests/Responsive/ResponsiveLayoutTests.cs
using Bunit;
using GitVisualizer.Components.Layout;
using GitVisualizer.Components.Sandbox;
using GitVisualizer.Interop;
using GitVisualizer.Services;
using GitVisualizer.Tests.Fakes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using MudBlazor.Services;

namespace GitVisualizer.Tests.Responsive;

/// <summary>
/// bUnit tests for Story 5.3 — Responsive Layout &amp; Reduced-Motion.
/// Covers AC1 (tablet CSS class), AC2/AC3 (mobile tab bar), AC4 (hamburger menu).
/// </summary>
public class ResponsiveLayoutTests : BunitContext
{
    private readonly FakeGitSimulatorService _git = new();
    private readonly FakeStorageMonitorService _storage = new();
    private readonly FakeGraphRenderService _fakeRenderService = new();
    private readonly FakeThemeService _fakeThemeService = new();
    private readonly FakeLocalStorageService _localStorage = new();

    public ResponsiveLayoutTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddMudServices();
        Services.AddSingleton<IGitSimulatorService>(_git);
        Services.AddSingleton<IStorageMonitorService>(_storage);
        Services.AddSingleton<IGraphRenderService>(_fakeRenderService);
        Services.AddSingleton<IThemeService>(_fakeThemeService);
        Services.AddSingleton<ILocalStorageService>(_localStorage);
        Services.AddSingleton<GraphRendererJsInterop>();
        Services.AddSingleton<ViewportJsInterop>();
    }

    protected override void Dispose(bool disposing)
    {
        try { base.Dispose(disposing); }
        catch (InvalidOperationException) { }
        catch (AggregateException ex) when (ex.InnerExceptions.All(e => e is InvalidOperationException)) { }
    }

    // ── AC1: Desktop — SplitPane rendered (isMobile returns false by default in Loose mode) ─────

    [Fact]
    public void SandboxPage_OnDesktop_RendersSplitPane()
    {
        // Register SplitPaneJsInterop needed by SplitPane (rendered on initial desktop layout)
        Services.AddSingleton<SplitPaneJsInterop>();

        var cut = Render<SandboxPage>();

        var splitPane = cut.FindComponents<SplitPane>();
        Assert.NotEmpty(splitPane);
    }

    // ── AC2: Mobile — MobileTabBar rendered when isMobile() returns true ─────────────────────

    [Fact]
    public void SandboxPage_OnMobile_RendersMobileTabBar()
    {
        // SplitPane is rendered on the first synchronous render (before OnAfterRender fires)
        Services.AddSingleton<SplitPaneJsInterop>();

        var moduleInterop = JSInterop.SetupModule("./js/viewport-interop.js");
        moduleInterop.Mode = JSRuntimeMode.Loose;
        moduleInterop.Setup<bool>("isMobile").SetResult(true);

        var cut = Render<SandboxPage>();

        cut.WaitForAssertion(() =>
        {
            var tabBar = cut.FindComponents<GitVisualizer.Components.Mobile.MobileTabBar>();
            Assert.NotEmpty(tabBar);
        }, timeout: TimeSpan.FromSeconds(2));
    }

    // ── AC3: Mobile — default active tab is Terminal ─────────────────────────────────────────

    [Fact]
    public void SandboxPage_OnMobile_DefaultActiveTabIsTerminal()
    {
        Services.AddSingleton<SplitPaneJsInterop>();

        var moduleInterop = JSInterop.SetupModule("./js/viewport-interop.js");
        moduleInterop.Mode = JSRuntimeMode.Loose;
        moduleInterop.Setup<bool>("isMobile").SetResult(true);

        var cut = Render<SandboxPage>();

        cut.WaitForAssertion(() =>
        {
            var tabBar = cut.FindComponent<GitVisualizer.Components.Mobile.MobileTabBar>();
            Assert.Equal("Terminal", tabBar.Instance.ActiveTab);
        }, timeout: TimeSpan.FromSeconds(2));
    }

    // ── AC3: Mobile — switching to Graph tab shows graph panel ───────────────────────────────

    [Fact]
    public void SandboxPage_OnMobile_SwitchingToGraphTabShowsGraph()
    {
        Services.AddSingleton<SplitPaneJsInterop>();

        var moduleInterop = JSInterop.SetupModule("./js/viewport-interop.js");
        moduleInterop.Mode = JSRuntimeMode.Loose;
        moduleInterop.Setup<bool>("isMobile").SetResult(true);

        var cut = Render<SandboxPage>();

        // Wait for mobile layout to render
        cut.WaitForAssertion(() =>
            Assert.NotEmpty(cut.FindComponents<GitVisualizer.Components.Mobile.MobileTabBar>()),
            timeout: TimeSpan.FromSeconds(2));

        // Click the Graph tab button
        var graphTabBtn = cut.FindAll("button[role='tab']")
            .FirstOrDefault(b => b.TextContent.Trim() == "Graph");
        Assert.NotNull(graphTabBtn);
        graphTabBtn.Click();

        // Graph panel should now be rendered
        cut.WaitForAssertion(() =>
        {
            var graphPanel = cut.Find("#graph-panel");
            Assert.NotNull(graphPanel);
        }, timeout: TimeSpan.FromSeconds(2));
    }

    // ── AC4: MainLayout hamburger menu element exists in DOM ─────────────────────────────────

    [Fact]
    public void MainLayout_HasHamburgerMenu_ForMobile()
    {
        Services.AddSingleton<IServiceWorkerUpdateService>(new FakeServiceWorkerUpdateService());

        Render<MudBlazor.MudPopoverProvider>();
        var cut = Render<MainLayout>();

        // .toolbar-mobile-only div should exist in the DOM (visible via CSS on mobile)
        var mobileDiv = cut.Find(".toolbar-mobile-only");
        Assert.NotNull(mobileDiv);
    }
}
