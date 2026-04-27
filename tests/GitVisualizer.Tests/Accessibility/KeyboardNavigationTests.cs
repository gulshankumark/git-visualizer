// tests/GitVisualizer.Tests/Accessibility/KeyboardNavigationTests.cs
using Bunit;
using GitVisualizer.Components.Common;
using GitVisualizer.Components.Sandbox;
using GitVisualizer.Interop;
using GitVisualizer.Services;
using GitVisualizer.Tests.Fakes;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using MudBlazor.Services;
using System.IO;

namespace GitVisualizer.Tests.Accessibility;

/// <summary>
/// bUnit tests for Story 5.1 — Keyboard Navigation &amp; Focus Management.
/// Covers AC1 (tab order), AC2 (focus outline CSS), AC4 (focus recovery + history nav).
/// </summary>
public class KeyboardNavigationTests : BunitContext
{
    private readonly FakeGitSimulatorService _git = new();
    private readonly FakeStorageMonitorService _storage = new();
    private readonly FakeLocalStorageService _localStorage = new();

    public KeyboardNavigationTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddMudServices();
        Services.AddSingleton<IGitSimulatorService>(_git);
        Services.AddSingleton<IStorageMonitorService>(_storage);
        Services.AddSingleton<ILocalStorageService>(_localStorage);
        Services.AddTransient<SplitPaneJsInterop>();
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

    // ── AC2: Focus outline CSS ────────────────────────────────────────────────

    [Fact]
    public void AppCss_Contains_FocusVisibleOutlineRule()
    {
        var cssPath = Path.Combine(GetRepoRoot(), "src/GitVisualizer/wwwroot/css/app.css");
        var css = File.ReadAllText(cssPath);

        Assert.Contains(":focus-visible", css);
        Assert.Contains("#3FB950", css);   // dark-mode green
        Assert.Contains("#2DA44E", css);   // light-mode green
    }

    [Fact]
    public void AppCss_FocusOutline_Is2pxSolid()
    {
        var cssPath = Path.Combine(GetRepoRoot(), "src/GitVisualizer/wwwroot/css/app.css");
        var css = File.ReadAllText(cssPath);

        Assert.Contains("2px solid", css);
        Assert.Contains("outline-offset", css);
    }

    [Fact]
    public void AppCss_DoesNotContain_OutlineNoneForAllElements()
    {
        var cssPath = Path.Combine(GetRepoRoot(), "src/GitVisualizer/wwwroot/css/app.css");
        var css = File.ReadAllText(cssPath);

        // Ensure we don't globally suppress focus outlines — only allow it for :not(:focus-visible)
        Assert.DoesNotContain("*{outline:none", css.Replace(" ", ""));
        Assert.DoesNotContain("*{ outline: none", css);
    }

    // ── AC1: Tab order — split handle ─────────────────────────────────────────

    [Fact]
    public void SplitPane_DragHandle_HasTabIndexZero()
    {
        var cut = Render<SplitPane>();

        var handle = cut.Find(".split-handle");
        Assert.Equal("0", handle.GetAttribute("tabindex"));
    }

    [Fact]
    public void SplitPane_DragHandle_HasRoleSeparator()
    {
        var cut = Render<SplitPane>();

        var handle = cut.Find(".split-handle");
        Assert.Equal("separator", handle.GetAttribute("role"));
    }

    [Fact]
    public void SplitPane_DragHandle_HasAriaLabel()
    {
        var cut = Render<SplitPane>();

        var handle = cut.Find(".split-handle");
        Assert.NotNull(handle.GetAttribute("aria-label"));
        Assert.NotEmpty(handle.GetAttribute("aria-label")!);
    }

    // ── AC1: Tab order — terminal input ──────────────────────────────────────

    [Fact]
    public void TerminalPanel_Input_HasAriaLabel()
    {
        var cut = Render<TerminalPanel>();

        var input = cut.Find("textarea");
        Assert.NotNull(input.GetAttribute("aria-label"));
    }

    // ── AC4: Focus recovery — input cleared after command submission ──────────

    [Fact]
    public void TerminalPanel_AfterEnter_InputIsCleared()
    {
        var cut = Render<TerminalPanel>();
        var textarea = cut.Find("textarea");
        textarea.TriggerEvent("oninput", new ChangeEventArgs { Value = "git status" });
        textarea.TriggerEvent("onkeydown", new KeyboardEventArgs { Key = "Enter" });

        Assert.DoesNotContain("git status", cut.Find("textarea").GetAttribute("value") ?? "");
    }

    // ── AC4: History navigation does not change document focus ────────────────

    [Fact]
    public void TerminalPanel_ArrowUp_NavigatesHistory_WithoutLeavingInput()
    {
        _git.AddHistoryEntry("git init", new Models.CommandResult(true, "", null, null, null));

        var cut = Render<TerminalPanel>();
        var textarea = cut.Find("textarea");

        // Arrow keys should change textarea value; input stays focused (JS side-effect)
        textarea.TriggerEvent("onkeydown", new KeyboardEventArgs { Key = "ArrowUp" });

        // Verify history value appears in the textarea
        Assert.Contains("git init", cut.Find("textarea").OuterHtml);
    }

    [Fact]
    public void TerminalPanel_ArrowDown_AfterArrowUp_RestoresDraftInput()
    {
        _git.AddHistoryEntry("git commit -m \"test\"", new Models.CommandResult(true, "", null, null, null));

        var cut = Render<TerminalPanel>();
        var textarea = cut.Find("textarea");
        textarea.TriggerEvent("oninput", new ChangeEventArgs { Value = "draft text" });
        textarea.TriggerEvent("onkeydown", new KeyboardEventArgs { Key = "ArrowUp" });
        textarea.TriggerEvent("onkeydown", new KeyboardEventArgs { Key = "ArrowDown" });

        // After ArrowDown back to start, draft is restored
        Assert.Contains("draft text", cut.Find("textarea").OuterHtml);
    }

    // ── AC3: ResetConfirmDialog — accessible dialog structure ─────────────────

    [Fact]
    public void ResetConfirmDialog_SourceFile_HasAutoFocusOnCancelButton()
    {
        var path = Path.Combine(GetRepoRoot(), "src/GitVisualizer/Components/Common/ResetConfirmDialog.razor");
        var source = File.ReadAllText(path);

        // Cancel button should have AutoFocus to receive focus when dialog opens
        Assert.Contains("AutoFocus=\"true\"", source);
        Assert.Contains("Cancel", source);
    }

    [Fact]
    public void StorageFullDialog_SourceFile_HasResetNowButton()
    {
        var path = Path.Combine(GetRepoRoot(), "src/GitVisualizer/Components/Common/StorageFullDialog.razor");
        var source = File.ReadAllText(path);

        Assert.Contains("Reset Now", source);
    }
}
