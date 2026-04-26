namespace GitVisualizer.Models;

/// <summary>
/// Represents current storage usage status and threshold states.
/// </summary>
public record StorageStatus(
    long UsageBytes,
    long QuotaBytes,
    double UsagePercent,
    bool IsWarning,  // True when usage >= 45MB
    bool IsBlocked   // True when usage >= 50MB
);
