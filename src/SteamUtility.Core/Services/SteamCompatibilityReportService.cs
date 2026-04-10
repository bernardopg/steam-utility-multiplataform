using SteamUtility.Core.Models;

namespace SteamUtility.Core.Services;

public sealed class SteamCompatibilityReportService
{
    private readonly SteamLibraryScanner _libraryScanner = new();
    private readonly SteamCompatDataScanner _compatDataScanner = new();
    private readonly SteamConfigCompatibilityParser _configParser = new();

    public IReadOnlyList<SteamCompatibilityReportEntry> Build(SteamInstallation installation)
    {
        var apps = _libraryScanner.ScanInstalledApps(installation);
        var compatData = _compatDataScanner.Scan(installation).ToDictionary(static item => item.AppId);
        var configPath = Path.Combine(installation.RootPath, "config", "config.vdf");
        var assignments = _configParser.Parse(configPath).ToDictionary(static item => item.AppId);

        var appIds = new SortedSet<int>(apps.Select(static app => app.AppId));
        appIds.UnionWith(compatData.Keys);
        appIds.UnionWith(assignments.Keys);

        var appLookup = apps.ToDictionary(static app => app.AppId);
        var results = new List<SteamCompatibilityReportEntry>();

        foreach (var appId in appIds)
        {
            appLookup.TryGetValue(appId, out var app);
            compatData.TryGetValue(appId, out var compat);
            assignments.TryGetValue(appId, out var assignment);

            results.Add(new SteamCompatibilityReportEntry(
                AppId: appId,
                Name: app?.Name ?? $"App {appId}",
                InstallDirectory: app?.InstallDirectory,
                HasCompatData: compat is not null,
                AssignedTool: assignment?.ToolName,
                AssignedToolPriority: assignment?.ToolPriority,
                AssignedToolConfig: assignment?.ToolConfig));
        }

        return results;
    }
}
