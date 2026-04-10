namespace SteamUtility.Core.Models;

public sealed record SteamCompatibilityReportEntry(
    int AppId,
    string Name,
    string? InstallDirectory,
    bool HasCompatData,
    string? AssignedTool,
    string? AssignedToolPriority,
    string? AssignedToolConfig);
