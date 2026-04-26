// src/GitVisualizer/Services/IGraphRenderService.cs
using GitVisualizer.Models;

namespace GitVisualizer.Services;

public interface IGraphRenderService
{
    /// <summary>Convert repo state to Mermaid gitGraph syntax. Returns empty string if no graph data.</summary>
    string ToMermaidSyntax(RepoState state);

    /// <summary>Build an accessible aria-label describing the current repo state.</summary>
    string BuildAriaLabel(RepoState? state);
}
