namespace SteamUtility.Core.Models;

public sealed record SteamAppCompatibilityAssignment(
    int AppId,
    string ToolName,
    string? ToolPriority,
    string? ToolConfig);
