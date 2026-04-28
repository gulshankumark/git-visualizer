using System.Text.Json;
using Xunit;

namespace GitVisualizer.Tests.PWA;

/// <summary>
/// End-to-end tests for PWA installation prompt and standalone mode.
/// Verifies manifest is present, valid, and install criteria are met.
/// Tests focus on file validation and accessibility rather than browser automation.
/// </summary>
public class InstallPromptTests
{
    private string GetManifestPath()
    {
        // When running from dotnet test, need to navigate from the bin directory to the solution root
        var binDir = Path.GetDirectoryName(typeof(InstallPromptTests).Assembly.Location);
        var solutionRoot = Path.Combine(binDir ?? "", "..", "..", "..", "..", "..");  // Navigate up 5 levels: net10.0 → Release → bin → TestProject → tests → solution root

        var possibleRoots = new[]
        {
            Environment.CurrentDirectory, // Current working directory (usually solution root when running dotnet test)
            Path.GetFullPath(solutionRoot),  // Solution root from assembly location
        };

        foreach (var root in possibleRoots)
        {
            var path = Path.Combine(root ?? "", "src", "GitVisualizer", "wwwroot", "manifest.json");
            if (File.Exists(path))
                return path;
        }

        return Path.Combine(Environment.CurrentDirectory, "src", "GitVisualizer", "wwwroot", "manifest.json");
    }

    private string GetIndexHtmlPath()
    {
        // When running from dotnet test, need to navigate from the bin directory to the solution root
        var binDir = Path.GetDirectoryName(typeof(InstallPromptTests).Assembly.Location);
        var solutionRoot = Path.Combine(binDir ?? "", "..", "..", "..", "..", "..");  // Navigate up 5 levels: net10.0 → Release → bin → TestProject → tests → solution root

        var possibleRoots = new[]
        {
            Environment.CurrentDirectory, // Current working directory
            Path.GetFullPath(solutionRoot),  // Solution root from assembly location
        };

        foreach (var root in possibleRoots)
        {
            var path = Path.Combine(root ?? "", "src", "GitVisualizer", "wwwroot", "index.html");
            if (File.Exists(path))
                return path;
        }

        return Path.Combine(Environment.CurrentDirectory, "src", "GitVisualizer", "wwwroot", "index.html");
    }

    private string GetLayoutPath()
    {
        // When running from dotnet test, need to navigate from the bin directory to the solution root
        var binDir = Path.GetDirectoryName(typeof(InstallPromptTests).Assembly.Location);
        var solutionRoot = Path.Combine(binDir ?? "", "..", "..", "..", "..", "..");  // Navigate up 5 levels: net10.0 → Release → bin → TestProject → tests → solution root

        var possibleRoots = new[]
        {
            Environment.CurrentDirectory, // Current working directory
            Path.GetFullPath(solutionRoot),  // Solution root from assembly location
        };

        foreach (var root in possibleRoots)
        {
            var path = Path.Combine(root ?? "", "src", "GitVisualizer", "Components", "Layout", "MainLayout.razor");
            if (File.Exists(path))
                return path;
        }

        return Path.Combine(Environment.CurrentDirectory, "src", "GitVisualizer", "Components", "Layout", "MainLayout.razor");
    }

    private string GetIcon192Path()
    {
        // When running from dotnet test, need to navigate from the bin directory to the solution root
        var binDir = Path.GetDirectoryName(typeof(InstallPromptTests).Assembly.Location);
        var solutionRoot = Path.Combine(binDir ?? "", "..", "..", "..", "..", "..");  // Navigate up 5 levels: net10.0 → Release → bin → TestProject → tests → solution root

        var possibleRoots = new[]
        {
            Environment.CurrentDirectory, // Current working directory
            Path.GetFullPath(solutionRoot),  // Solution root from assembly location
        };

        foreach (var root in possibleRoots)
        {
            var path = Path.Combine(root ?? "", "src", "GitVisualizer", "wwwroot", "img", "icon-192x192.png");
            if (File.Exists(path))
                return path;
        }

        return Path.Combine(Environment.CurrentDirectory, "src", "GitVisualizer", "wwwroot", "img", "icon-192x192.png");
    }

    private string GetIcon512Path()
    {
        // When running from dotnet test, need to navigate from the bin directory to the solution root
        var binDir = Path.GetDirectoryName(typeof(InstallPromptTests).Assembly.Location);
        var solutionRoot = Path.Combine(binDir ?? "", "..", "..", "..", "..", "..");  // Navigate up 5 levels: net10.0 → Release → bin → TestProject → tests → solution root

        var possibleRoots = new[]
        {
            Environment.CurrentDirectory, // Current working directory
            Path.GetFullPath(solutionRoot),  // Solution root from assembly location
        };

        foreach (var root in possibleRoots)
        {
            var path = Path.Combine(root ?? "", "src", "GitVisualizer", "wwwroot", "img", "icon-512x512.png");
            if (File.Exists(path))
                return path;
        }

        return Path.Combine(Environment.CurrentDirectory, "src", "GitVisualizer", "wwwroot", "img", "icon-512x512.png");
    }

    [Fact]
    [Trait("Category", "E2E")]
    public async Task ManifestLink_ShouldBePresent_InIndexHtml()
    {
        // Arrange
        var manifestPath = GetManifestPath();
        var indexHtmlPath = GetIndexHtmlPath();

        // Act
        var indexHtml = File.ReadAllText(indexHtmlPath);

        // Assert
        Assert.Contains("manifest.json", indexHtml);
        Assert.Contains("rel=\"manifest\"", indexHtml);
        Assert.True(File.Exists(manifestPath), "manifest.json should exist");
    }

    [Fact]
    [Trait("Category", "E2E")]
    public async Task Manifest_ShouldBeValidJson()
    {
        // Arrange
        var manifestPath = GetManifestPath();

        // Act
        var manifestJson = File.ReadAllText(manifestPath);
        var doc = JsonDocument.Parse(manifestJson);

        // Assert
        Assert.NotNull(doc);
        var root = doc.RootElement;
        Assert.True(root.TryGetProperty("name", out _), "Manifest should have name property");
        Assert.True(root.TryGetProperty("icons", out _), "Manifest should have icons property");
        Assert.True(root.TryGetProperty("display", out _), "Manifest should have display property");
    }

    [Fact]
    [Trait("Category", "E2E")]
    public async Task GitHubLink_ShouldHaveProperAttributes()
    {
        // Arrange
        var layoutPath = GetLayoutPath();

        // Act
        var layoutContent = File.ReadAllText(layoutPath);

        // Assert
        // Verify that GitHub link opens externally
        Assert.Contains("Target=\"_blank\"", layoutContent);
        Assert.Contains("https://github.com/gulshankumark/git-visualizer", layoutContent);
        // Verify rel="noopener noreferrer" for security
        Assert.Contains("rel=\"noopener noreferrer\"", layoutContent);
    }

    [Fact]
    [Trait("Category", "E2E")]
    public async Task IconFiles_ShouldExistAtCorrectPaths()
    {
        // Arrange
        var icon192Path = GetIcon192Path();
        var icon512Path = GetIcon512Path();

        // Act & Assert
        Assert.True(File.Exists(icon192Path), $"Icon should exist at {icon192Path}");
        Assert.True(File.Exists(icon512Path), $"Icon should exist at {icon512Path}");

        // Verify file sizes are reasonable for icons
        var icon192Size = new FileInfo(icon192Path).Length;
        var icon512Size = new FileInfo(icon512Path).Length;

        Assert.True(icon192Size > 100, "Icon files should have content");
        Assert.True(icon512Size > 100, "Icon files should have content");
    }

    [Fact]
    [Trait("Category", "E2E")]
    public async Task ManifestMetadata_ShouldMatchSpecification()
    {
        // Arrange
        var manifestPath = GetManifestPath();
        var manifestJson = File.ReadAllText(manifestPath);
        var doc = JsonDocument.Parse(manifestJson);
        var root = doc.RootElement;

        // Act & Assert
        // AC1: Manifest Configuration
        Assert.Equal("git-visualizer", root.GetProperty("name").GetString());
        Assert.Equal("GitViz", root.GetProperty("short_name").GetString());
        Assert.Equal("/git-visualizer/", root.GetProperty("start_url").GetString());
        Assert.Equal("standalone", root.GetProperty("display").GetString());
        Assert.Equal("#0D1117", root.GetProperty("background_color").GetString());
        Assert.Equal("#0D1117", root.GetProperty("theme_color").GetString());
        Assert.Equal("/git-visualizer/", root.GetProperty("scope").GetString());

        // Verify description exists
        var description = root.GetProperty("description").GetString();
        Assert.NotNull(description);
        Assert.NotEmpty(description);

        // Verify categories
        var categories = root.GetProperty("categories").EnumerateArray();
        var categoryList = categories.Select(c => c.GetString()).ToList();
        Assert.Contains("education", categoryList);
        Assert.Contains("productivity", categoryList);
    }

    [Fact]
    [Trait("Category", "E2E")]
    public async Task ManifestIcons_ShouldHaveCorrectSrcPaths()
    {
        // Arrange
        var manifestPath = GetManifestPath();
        var manifestJson = File.ReadAllText(manifestPath);
        var doc = JsonDocument.Parse(manifestJson);
        var root = doc.RootElement;

        // Act
        var icons = root.GetProperty("icons").EnumerateArray().ToList();

        // Assert
        var icon192 = icons.FirstOrDefault(i => 
            i.TryGetProperty("sizes", out var sizes) && sizes.GetString() == "192x192");
        var icon512 = icons.FirstOrDefault(i => 
            i.TryGetProperty("sizes", out var sizes) && sizes.GetString() == "512x512");

        Assert.True(icon192.TryGetProperty("src", out var src192), "Icon 192x192 should exist");
        Assert.True(icon512.TryGetProperty("src", out var src512), "Icon 512x512 should exist");

        Assert.Equal("/git-visualizer/img/icon-192x192.png", src192.GetString());
        Assert.Equal("/git-visualizer/img/icon-512x512.png", src512.GetString());
    }

    [Fact]
    [Trait("Category", "E2E")]
    public async Task ManifestIcons_ShouldBeMaskable()
    {
        // Arrange
        var manifestPath = GetManifestPath();
        var manifestJson = File.ReadAllText(manifestPath);
        var doc = JsonDocument.Parse(manifestJson);
        var root = doc.RootElement;

        // Act
        var icons = root.GetProperty("icons").EnumerateArray().ToList();

        // Assert
        foreach (var icon in icons)
        {
            Assert.True(icon.TryGetProperty("purpose", out var purposeProperty), "Icons should have purpose property");
            var purpose = purposeProperty.GetString();
            Assert.True(purpose!.Contains("maskable"), "Icons should support maskable purpose");
        }
    }

    [Fact]
    [Trait("Category", "E2E")]
    public async Task IconFiles_ShouldBePNGFormat()
    {
        // Arrange
        var icon192Path = GetIcon192Path();
        var icon512Path = GetIcon512Path();

        // Act
        var icon192Bytes = File.ReadAllBytes(icon192Path);
        var icon512Bytes = File.ReadAllBytes(icon512Path);

        // Assert - PNG magic number: 89 50 4E 47
        Assert.Equal(0x89, icon192Bytes[0]);
        Assert.Equal(0x50, icon192Bytes[1]);
        Assert.Equal(0x4E, icon192Bytes[2]);
        Assert.Equal(0x47, icon192Bytes[3]);

        Assert.Equal(0x89, icon512Bytes[0]);
        Assert.Equal(0x50, icon512Bytes[1]);
        Assert.Equal(0x4E, icon512Bytes[2]);
        Assert.Equal(0x47, icon512Bytes[3]);
    }

    [Fact]
    [Trait("Category", "E2E")]
    public async Task ServiceWorker_ShouldBeRegisteredForPWASupport()
    {
        // Arrange
        var indexHtmlPath = GetIndexHtmlPath();

        // Act
        var indexHtml = File.ReadAllText(indexHtmlPath);

        // Assert
        Assert.Contains("service-worker.js", indexHtml);
        Assert.Contains("navigator.serviceWorker.register", indexHtml);
    }

    [Fact]
    [Trait("Category", "E2E")]
    public async Task Manifest_OrientationShouldBeSet()
    {
        // Arrange
        var manifestPath = GetManifestPath();
        var manifestJson = File.ReadAllText(manifestPath);
        var doc = JsonDocument.Parse(manifestJson);
        var root = doc.RootElement;

        // Act & Assert
        Assert.True(root.TryGetProperty("orientation", out var orientationProperty), "Manifest should have orientation");
        Assert.Equal("portrait-primary", orientationProperty.GetString());
    }
}
