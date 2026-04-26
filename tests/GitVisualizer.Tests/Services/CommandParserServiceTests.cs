// tests/GitVisualizer.Tests/Services/CommandParserServiceTests.cs
using GitVisualizer.Services;

namespace GitVisualizer.Tests.Services;

public class CommandParserServiceTests
{
    private readonly CommandParserService _sut = new();

    [Fact]
    public void Parse_GitInit_ReturnsInitWithEmptyArgs()
    {
        var result = _sut.Parse("git init");

        Assert.Equal("init", result.Name);
        Assert.Empty(result.Args);
    }

    [Fact]
    public void Parse_GitCommitWithMessage_ReturnsCommitWithMessageArg()
    {
        var result = _sut.Parse("git commit -m \"my message\"");

        Assert.Equal("commit", result.Name);
        Assert.Equal("my message", result.Args["m"]);
    }

    [Fact]
    public void Parse_GitAdd_ReturnsAddWithFilepathArg()
    {
        var result = _sut.Parse("git add .");

        Assert.Equal("add", result.Name);
        Assert.Equal(".", result.Args["arg0"]);
    }

    [Fact]
    public void Parse_GitCheckoutWithB_ReturnsCheckoutWithBArg()
    {
        var result = _sut.Parse("git checkout -b feature");

        Assert.Equal("checkout", result.Name);
        Assert.Equal("feature", result.Args["b"]);
    }

    [Fact]
    public void Parse_GitHelp_ReturnsHelpCommand()
    {
        var result = _sut.Parse("git help");

        Assert.Equal("help", result.Name);
    }

    [Fact]
    public void Parse_CaseInsensitive_NormalizesToLowercase()
    {
        var result = _sut.Parse("GIT COMMIT -m \"msg\"");

        Assert.Equal("commit", result.Name);
        Assert.Equal("msg", result.Args["m"]);
    }

    [Fact]
    public void Parse_NonGitCommand_ReturnsEmptyName()
    {
        var result = _sut.Parse("npm install");

        Assert.Equal("", result.Name);
    }

    [Fact]
    public void Parse_EmptyInput_ReturnsEmptyName()
    {
        var result = _sut.Parse("   ");

        Assert.Equal("", result.Name);
        Assert.Empty(result.Args);
    }

    [Fact]
    public void Suggest_Typo_ReturnsCorrection()
    {
        var suggestion = _sut.Suggest("git comit -m \"x\"");

        Assert.Equal("git commit", suggestion);
    }

    [Fact]
    public void Suggest_UnknownCommandWithHighDistance_ReturnsNull()
    {
        var suggestion = _sut.Suggest("git xyz");

        Assert.Null(suggestion);
    }

    [Fact]
    public void Suggest_ValidCommand_ReturnsNull()
    {
        var suggestion = _sut.Suggest("git commit");

        Assert.Null(suggestion);
    }

    [Fact]
    public void Suggest_NonGitInput_ReturnsNull()
    {
        var suggestion = _sut.Suggest("npm install");

        Assert.Null(suggestion);
    }
}
