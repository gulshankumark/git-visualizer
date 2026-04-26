// src/GitVisualizer/Services/IGraphRenderService.cs
using GitVisualizer.Models;

namespace GitVisualizer.Services;

public interface IGraphRenderService
{
    /// <summary>Build a renderer-agnostic payload from current repo state. Returns null when there is nothing to render.</summary>
    GraphRenderPayload? BuildPayload(RepoState state);

    /// <summary>Build an accessible aria-label describing the current repo state.</summary>
    string BuildAriaLabel(RepoState? state);
}
