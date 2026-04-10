using SteamUtility.Core.Models;
using SteamUtility.Core.Vdf;

namespace SteamUtility.Core.Services;

public sealed class SteamConfigCompatibilityParser
{
    public IReadOnlyList<SteamAppCompatibilityAssignment> Parse(string configPath)
    {
        if (!File.Exists(configPath))
        {
            return [];
        }

        var root = SimpleVdfReader.Parse(File.ReadAllText(configPath));
        var compatToolMapping = root
            .GetChildren("InstallConfigStore").FirstOrDefault()?
            .GetChildren("Software").FirstOrDefault()?
            .GetChildren("Valve").FirstOrDefault()?
            .GetChildren("Steam").FirstOrDefault()?
            .GetChildren("CompatToolMapping").FirstOrDefault();

        if (compatToolMapping is null)
        {
            return [];
        }

        var results = new List<SteamAppCompatibilityAssignment>();

        foreach (var mapping in compatToolMapping.Children)
        {
            if (!int.TryParse(mapping.Key, out var appId))
            {
                continue;
            }

            var value = mapping.Value.FirstOrDefault();
            if (value is null)
            {
                continue;
            }

            var toolName = value.GetSingleValue("name");
            if (string.IsNullOrWhiteSpace(toolName))
            {
                continue;
            }

            results.Add(new SteamAppCompatibilityAssignment(
                AppId: appId,
                ToolName: toolName,
                ToolPriority: value.GetSingleValue("Priority"),
                ToolConfig: value.GetSingleValue("config")));
        }

        return results
            .OrderBy(static item => item.AppId)
            .ToArray();
    }
}
