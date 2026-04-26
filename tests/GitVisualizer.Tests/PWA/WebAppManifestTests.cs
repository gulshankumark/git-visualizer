using Xunit;
using System.Text.Json;
using System.IO;

namespace GitVisualizer.Tests.PWA;

/// <summary>Tests for Web App Manifest configuration (Story 4.2)</summary>
public class WebAppManifestTests
{
    private string GetManifestPath()
    {
        // Navigate from test output directory to source
        var currentDir = new DirectoryInfo(AppContext.BaseDirectory);
        while (currentDir != null && !File.Exists(Path.Combine(currentDir.FullName, "git-visualizer.sln")))
        {
            currentDir = currentDir.Parent;
        }
        if (currentDir == null)
        {
            throw new InvalidOperationException("Could not find repository root");
        }
        return Path.Combine(currentDir.FullName, "src/GitVisualizer/wwwroot/manifest.json");
    }

    private string GetIndexHtmlPath()
    {
        var currentDir = new DirectoryInfo(AppContext.BaseDirectory);
        while (currentDir != null && !File.Exists(Path.Combine(currentDir.FullName, "git-visualizer.sln")))
        {
            currentDir = currentDir.Parent;
        }
        if (currentDir == null)
        {
            throw new InvalidOperationException("Could not find repository root");
        }
        return Path.Combine(currentDir.FullName, "src/GitVisualizer/wwwroot/index.html");
    }

    private string GetIcon192Path()
    {
        var currentDir = new DirectoryInfo(AppContext.BaseDirectory);
        while (currentDir != null && !File.Exists(Path.Combine(currentDir.FullName, "git-visualizer.sln")))
        {
            currentDir = currentDir.Parent;
        }
        if (currentDir == null)
        {
            throw new InvalidOperationException("Could not find repository root");
        }
        return Path.Combine(currentDir.FullName, "src/GitVisualizer/wwwroot/img/icon-192x192.png");
    }

    private string GetIcon512Path()
    {
        var currentDir = new DirectoryInfo(AppContext.BaseDirectory);
        while (currentDir != null && !File.Exists(Path.Combine(currentDir.FullName, "git-visualizer.sln")))
        {
            currentDir = currentDir.Parent;
        }
        if (currentDir == null)
        {
            throw new InvalidOperationException("Could not find repository root");
        }
        return Path.Combine(currentDir.FullName, "src/GitVisualizer/wwwroot/img/icon-512x512.png");
    }

    [Fact]
    public void Manifest_Exists()
    {
        var path = GetManifestPath();
        Assert.True(File.Exists(path), $"Manifest file not found at {path}");
    }

    [Fact]
    public void Manifest_IsValidJson()
    {
        var path = GetManifestPath();
        var json = File.ReadAllText(path);
        var manifest = JsonSerializer.Deserialize<JsonElement>(json);
        Assert.NotEqual(default, manifest);
    }

    [Fact]
    public void Manifest_HasRequiredProperties()
    {
        var path = GetManifestPath();
        var json = File.ReadAllText(path);
        var manifest = JsonSerializer.Deserialize<JsonElement>(json);

        // Check all required properties
        Assert.True(manifest.TryGetProperty("name", out var name) && name.GetString() == "git-visualizer");
        Assert.True(manifest.TryGetProperty("short_name", out var shortName) && shortName.GetString() == "GitViz");
        Assert.True(manifest.TryGetProperty("start_url", out var startUrl) && startUrl.GetString() == "/git-visualizer/");
        Assert.True(manifest.TryGetProperty("display", out var display) && display.GetString() == "standalone");
        Assert.True(manifest.TryGetProperty("background_color", out var bgColor) && bgColor.GetString() == "#0D1117");
        Assert.True(manifest.TryGetProperty("theme_color", out var themeColor) && themeColor.GetString() == "#0D1117");
        Assert.True(manifest.TryGetProperty("scope", out var scope) && scope.GetString() == "/git-visualizer/");
    }

    [Fact]
    public void Manifest_HasApplicationIcons()
    {
        var path = GetManifestPath();
        var json = File.ReadAllText(path);
        var manifest = JsonSerializer.Deserialize<JsonElement>(json);

        Assert.True(manifest.TryGetProperty("icons", out var icons));
        Assert.Equal(2, icons.GetArrayLength());

        // Check 192x192 icon
        var icon192 = icons[0];
        Assert.Equal("/git-visualizer/img/icon-192x192.png", icon192.GetProperty("src").GetString());
        Assert.Equal("192x192", icon192.GetProperty("sizes").GetString());
        Assert.Equal("image/png", icon192.GetProperty("type").GetString());

        // Check 512x512 icon
        var icon512 = icons[1];
        Assert.Equal("/git-visualizer/img/icon-512x512.png", icon512.GetProperty("src").GetString());
        Assert.Equal("512x512", icon512.GetProperty("sizes").GetString());
        Assert.Equal("image/png", icon512.GetProperty("type").GetString());
    }

    [Fact]
    public void ApplicationIcons_Exist()
    {
        var icon192Path = GetIcon192Path();
        var icon512Path = GetIcon512Path();

        Assert.True(File.Exists(icon192Path), $"192x192 icon not found at {icon192Path}");
        Assert.True(File.Exists(icon512Path), $"512x512 icon not found at {icon512Path}");
    }

    [Fact]
    public void IndexHtml_ContainsManifestLink()
    {
        var indexPath = GetIndexHtmlPath();
        var html = File.ReadAllText(indexPath);

        Assert.Contains("""<link href="manifest.json" rel="manifest" />""", html);
    }

    [Fact]
    public void IndexHtml_ContainsAppleTouchIcons()
    {
        var indexPath = GetIndexHtmlPath();
        var html = File.ReadAllText(indexPath);

        Assert.Contains("""<link rel="apple-touch-icon" sizes="512x512" href="icon-512.png" />""", html);
        Assert.Contains("""<link rel="apple-touch-icon" sizes="192x192" href="icon-192.png" />""", html);
    }

    [Fact]
    public void IndexHtml_ContainsFavicon()
    {
        var indexPath = GetIndexHtmlPath();
        var html = File.ReadAllText(indexPath);

        Assert.Contains("""<link rel="icon" type="image/png" href="favicon.png" />""", html);
    }
}

