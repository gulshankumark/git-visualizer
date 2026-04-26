// tests/GitVisualizer.Tests/Services/GitSimulatorServiceTests.cs
using Bunit;
using GitVisualizer.Interop;
using GitVisualizer.Services;
using GitVisualizer.Tests.Fakes;
using System.Text.Json;

namespace GitVisualizer.Tests.Services;

public class GitSimulatorServiceTests : TestContext
{
    // Same Dispose override as AppShellTests — tolerates MudPopoverProvider teardown
    protected override void Dispose(bool disposing)
    {
        try { base.Dispose(disposing); }
        catch (InvalidOperationException) { }
        catch (AggregateException ex) when (ex.InnerExceptions.All(e => e is InvalidOperationException)) { }
    }

    [Fact]
    public async Task InitRepoAsync_WithMockedJs_ReturnsSuccessWithUpdatedState()
    {
        var moduleInterop = JSInterop.SetupModule("./js/git-interop.js");
        moduleInterop
            .Setup<JsonElement>("gitInit")
            .SetResult(JsonDocument.Parse("""{"success":true,"message":"Initialized empty Git repository in /"}""").RootElement);

        var sut = new GitSimulatorService(
            new GitJsInterop(JSInterop.JSRuntime),
            new FakeCommandParserService(),
            new FakeSessionStorageService());

        var result = await sut.InitRepoAsync();

        Assert.True(result.Success);
        Assert.NotNull(result.UpdatedState);
        Assert.True(result.UpdatedState.IsInitialized);
        Assert.Equal("main", result.UpdatedState.CurrentBranch);
    }

    [Fact]
    public async Task ExecuteCommandAsync_GitInit_RoutesToInitAndAppendsHistory()
    {
        var moduleInterop = JSInterop.SetupModule("./js/git-interop.js");
        moduleInterop
            .Setup<JsonElement>("gitInit")
            .SetResult(JsonDocument.Parse("""{"success":true,"message":"Initialized."}""").RootElement);
        moduleInterop
            .Setup<JsonElement>("gitGetGraph")
            .SetResult(JsonDocument.Parse("""{"success":true,"branches":[],"headBranch":"main","commits":[],"branchTips":{}}""").RootElement);

        var sut = new GitSimulatorService(
            new GitJsInterop(JSInterop.JSRuntime),
            new FakeCommandParserService(),
            new FakeSessionStorageService());

        var result = await sut.ExecuteCommandAsync("git init");

        Assert.True(result.Success);
        Assert.Single(sut.CommandHistory);
    }

    [Fact]
    public async Task ExecuteCommandAsync_UnknownCommand_ReturnsFriendlyErrorWithSuggestedFix()
    {
        var sut = new GitSimulatorService(
            new GitJsInterop(JSInterop.JSRuntime),
            new FakeCommandParserService(),
            new FakeSessionStorageService());

        var result = await sut.ExecuteCommandAsync("npm install");

        Assert.False(result.Success);
        Assert.NotNull(result.ErrorMessage);
        Assert.Contains("git help", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
        Assert.Equal("git help", result.SuggestedFix);
    }

    [Fact]
    public async Task ExecuteCommandAsync_TypoGitSubcommand_ReturnsSuggestion()
    {
        var sut = new GitSimulatorService(
            new GitJsInterop(JSInterop.JSRuntime),
            new CommandParserService(),
            new FakeSessionStorageService());

        var result = await sut.ExecuteCommandAsync("git comit");

        Assert.False(result.Success);
        Assert.NotNull(result.ErrorMessage);
        Assert.Contains("Did you mean", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
        Assert.Equal("git commit", result.SuggestedFix);
    }
}

