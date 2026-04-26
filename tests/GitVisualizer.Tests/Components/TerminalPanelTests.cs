// tests/GitVisualizer.Tests/Components/TerminalPanelTests.cs
using Bunit;
using GitVisualizer.Components.Sandbox;
using GitVisualizer.Models;
using GitVisualizer.Services;
using GitVisualizer.Tests.Fakes;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using MudBlazor.Services;

namespace GitVisualizer.Tests.Components;

/// <summary>bUnit tests for the TerminalPanel component.</summary>
public class TerminalPanelTests : BunitContext
{
    private readonly FakeGitSimulatorService _git = new();
    private readonly FakeStorageMonitorService _storage = new();

    public TerminalPanelTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        Services.AddMudServices();
        Services.AddSingleton<IGitSimulatorService>(_git);
        Services.AddSingleton<IStorageMonitorService>(_storage);
    }

    protected override void Dispose(bool disposing)
    {
        try { base.Dispose(disposing); }
        catch (InvalidOperationException) { }
        catch (AggregateException ex) when (ex.InnerExceptions.All(e => e is InvalidOperationException)) { }
    }

    // ── AC1 & AC4: Accessible structure ─────────────────────────────────────

    [Fact]
    public void TerminalPanel_OnRender_HasAccessibleInputAndLiveRegion()
    {
        var cut = Render<TerminalPanel>();
        Assert.NotNull(cut.Find("[aria-label='Git command input']"));
        Assert.NotNull(cut.Find("[aria-live='polite']"));
    }

    // ── AC2: Progress bar (loading state) ────────────────────────────────────

    [Fact]
    public void TerminalPanel_WhenIdle_ProgressBarIsHidden()
    {
        var cut = Render<TerminalPanel>();
        Assert.Empty(cut.FindAll(".terminal-progress"));
    }

    [Fact]
    public void TerminalPanel_WhenProcessing_ProgressBarIsVisible()
    {
        _git.IsProcessing = true;
        var cut = Render<TerminalPanel>();
        Assert.NotNull(cut.Find(".terminal-progress"));
    }

    // ── AC2: Output rendering ────────────────────────────────────────────────

    [Fact]
    public void TerminalPanel_WithSuccessEntry_ShowsCommandAndOutput()
    {
        _git.AddHistoryEntry("git init",
            new CommandResult(true, "Initialized empty Git repository", null, null, null));

        var cut = Render<TerminalPanel>();
        Assert.Contains("git init", cut.Markup);
        Assert.Contains("Initialized empty Git repository", cut.Markup);
    }

    [Fact]
    public void TerminalPanel_WithFailedEntry_ShowsWarningAlert()
    {
        _git.AddHistoryEntry("git oomit",
            new CommandResult(false, "", "'git oomit' is not a command.", "Did you mean 'git commit'?", null));

        var cut = Render<TerminalPanel>();
        Assert.Contains("terminal-alert", cut.Markup);
        Assert.Contains("'git oomit' is not a command.", cut.Markup);
        Assert.Contains("Did you mean 'git commit'?", cut.Markup);
    }

    // ── AC2: Command dispatch ────────────────────────────────────────────────

    [Fact]
    public void TerminalPanel_EnterKey_DispatchesCommandToService()
    {
        var cut = Render<TerminalPanel>();
        var textarea = cut.Find("textarea");
        textarea.TriggerEvent("oninput", new ChangeEventArgs { Value = "git status" });
        textarea.TriggerEvent("onkeydown", new KeyboardEventArgs { Key = "Enter" });

        Assert.Contains("git status", _git.ExecutedCommands);
    }

    [Fact]
    public void TerminalPanel_EmptyInput_DoesNotDispatchCommand()
    {
        var cut = Render<TerminalPanel>();
        cut.Find("textarea").TriggerEvent("onkeydown", new KeyboardEventArgs { Key = "Enter" });

        Assert.Empty(_git.ExecutedCommands);
    }

    // ── AC6: Shift+Enter does not submit ─────────────────────────────────────

    [Fact]
    public void TerminalPanel_ShiftEnter_DoesNotSubmitCommand()
    {
        var cut = Render<TerminalPanel>();
        var textarea = cut.Find("textarea");
        textarea.TriggerEvent("oninput", new ChangeEventArgs { Value = "some text" });
        textarea.TriggerEvent("onkeydown", new KeyboardEventArgs { Key = "Enter", ShiftKey = true });

        Assert.Empty(_git.ExecutedCommands);
    }

    [Fact]
    public void TerminalPanel_ShiftEnter_InsertsNewlineIntoInput()
    {
        var cut = Render<TerminalPanel>();
        var textarea = cut.Find("textarea");
        textarea.TriggerEvent("oninput", new ChangeEventArgs { Value = "line one" });
        textarea.TriggerEvent("onkeydown", new KeyboardEventArgs { Key = "Enter", ShiftKey = true });

        Assert.Contains("\n", cut.Find("textarea").GetAttribute("value") ?? cut.Find("textarea").OuterHtml);
    }

    // ── AC1: Focus regain after command execution ─────────────────────────────

    [Fact]
    public void TerminalPanel_AfterCommandExecution_InputIsCleared()
    {
        var cut = Render<TerminalPanel>();
        var textarea = cut.Find("textarea");
        textarea.TriggerEvent("oninput", new ChangeEventArgs { Value = "git status" });
        textarea.TriggerEvent("onkeydown", new KeyboardEventArgs { Key = "Enter" });

        // Input must be cleared after submission (focus regain is a JS side-effect verified separately)
        Assert.DoesNotContain("git status", cut.Find("textarea").GetAttribute("value") ?? "");
    }

    // ── AC5: clear command & Clear button ────────────────────────────────────

    [Fact]
    public void TerminalPanel_ClearCommand_CallsClearAsync()
    {
        var cut = Render<TerminalPanel>();
        var textarea = cut.Find("textarea");
        textarea.TriggerEvent("oninput", new ChangeEventArgs { Value = "clear" });
        textarea.TriggerEvent("onkeydown", new KeyboardEventArgs { Key = "Enter" });

        Assert.Equal(1, _git.ClearCount);
        Assert.Equal(0, _git.ExecutedCommands.Count); // 'clear' is NOT forwarded to service
    }

    [Fact]
    public void TerminalPanel_ClearButton_CallsClearAsync()
    {
        var cut = Render<TerminalPanel>();
        cut.Find("[aria-label='Clear terminal output']").Click();

        Assert.Equal(1, _git.ClearCount);
    }

    // ── AC3: History navigation ──────────────────────────────────────────────


    [Fact]
    public void TerminalPanel_ArrowUp_FillsInputWithMostRecentCommand()
    {
        _git.AddHistoryEntry("git init",   new CommandResult(true, "", null, null, null));
        _git.AddHistoryEntry("git status", new CommandResult(true, "", null, null, null));

        var cut = Render<TerminalPanel>();
        cut.Find("textarea").TriggerEvent("onkeydown", new KeyboardEventArgs { Key = "ArrowUp" });

        // most-recent-first: first ArrowUp → "git status"
        Assert.Contains("git status", cut.Find("textarea").OuterHtml);
    }

    [Fact]
    public void TerminalPanel_ArrowUp_TwiceReachesOlderCommand()
    {
        _git.AddHistoryEntry("git init",   new CommandResult(true, "", null, null, null));
        _git.AddHistoryEntry("git status", new CommandResult(true, "", null, null, null));

        var cut = Render<TerminalPanel>();
        var textarea = cut.Find("textarea");
        textarea.TriggerEvent("onkeydown", new KeyboardEventArgs { Key = "ArrowUp" });
        textarea.TriggerEvent("onkeydown", new KeyboardEventArgs { Key = "ArrowUp" });

        Assert.Contains("git init", cut.Find("textarea").OuterHtml);
    }

    [Fact]
    public void TerminalPanel_ArrowDown_AfterArrowUp_RestoresSavedInput()
    {
        _git.AddHistoryEntry("git init", new CommandResult(true, "", null, null, null));

        var cut = Render<TerminalPanel>();
        var textarea = cut.Find("textarea");
        textarea.TriggerEvent("oninput",   new ChangeEventArgs    { Value = "my draft" });
        textarea.TriggerEvent("onkeydown", new KeyboardEventArgs  { Key = "ArrowUp" });
        textarea.TriggerEvent("onkeydown", new KeyboardEventArgs  { Key = "ArrowDown" });

        Assert.Contains("my draft", cut.Find("textarea").OuterHtml);
    }

    [Fact]
    public void TerminalPanel_ArrowUp_NoHistory_DoesNothing()
    {
        var cut = Render<TerminalPanel>();
        var textarea = cut.Find("textarea");
        textarea.TriggerEvent("oninput",   new ChangeEventArgs   { Value = "hello" });
        textarea.TriggerEvent("onkeydown", new KeyboardEventArgs { Key = "ArrowUp" });

        // No history → input unchanged
        Assert.Contains("hello", cut.Find("textarea").OuterHtml);
    }

    // ── Help Overlay Tests ────────────────────────────────────────────────────

    [Fact]
    public void TerminalPanel_GitHelpCommand_InvokesOnHelpRequestedCallback()
    {
        var helpRequested = false;
        var cut = Render<TerminalPanel>(p =>
            p.Add(c => c.OnHelpRequested, EventCallback.Factory.Create(this, async () =>
            {
                helpRequested = true;
                await Task.CompletedTask;
            }))
        );

        var textarea = cut.Find("textarea");
        textarea.TriggerEvent("oninput", new ChangeEventArgs { Value = "git help" });
        textarea.TriggerEvent("onkeydown", new KeyboardEventArgs { Key = "Enter" });

        Assert.True(helpRequested);
    }

    [Fact]
    public void TerminalPanel_GitHelpCommand_DoesNotExecuteAsRegularCommand()
    {
        var cut = Render<TerminalPanel>();
        var textarea = cut.Find("textarea");
        textarea.TriggerEvent("oninput", new ChangeEventArgs { Value = "git help" });
        textarea.TriggerEvent("onkeydown", new KeyboardEventArgs { Key = "Enter" });

        Assert.Empty(_git.ExecutedCommands); // 'git help' NOT forwarded to service
    }

    [Fact]
    public void TerminalPanel_GitHelpCommand_ClearsInput()
    {
        var cut = Render<TerminalPanel>();
        var textarea = cut.Find("textarea");
        textarea.TriggerEvent("oninput", new ChangeEventArgs { Value = "git help" });
        textarea.TriggerEvent("onkeydown", new KeyboardEventArgs { Key = "Enter" });

        Assert.DoesNotContain("git help", cut.Find("textarea").GetAttribute("value") ?? "");
    }
}
