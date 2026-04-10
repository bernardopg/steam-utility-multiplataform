using Microsoft.Win32;
using SteamUtility.Core.Abstractions;
using System.Runtime.Versioning;

namespace SteamUtility.Core.Services;

[SupportedOSPlatform("windows")]
public sealed class WindowsSteamLocator : ISteamLocator
{
    private static readonly (string RegistryPath, string ValueName)[] RegistryCandidates =
    [
        (@"HKEY_LOCAL_MACHINE\Software\Valve\Steam", "InstallPath"),
        (@"HKEY_LOCAL_MACHINE\Software\WOW6432Node\Valve\Steam", "InstallPath"),
        (@"HKEY_CURRENT_USER\Software\Valve\Steam", "SteamPath"),
        (@"HKEY_CURRENT_USER\Software\Valve\Steam", "InstallPath")
    ];

    public string? TryGetSteamRoot()
    {
        foreach (var (registryPath, valueName) in RegistryCandidates)
        {
            var candidate = Registry.GetValue(registryPath, valueName, null) as string;
            if (!string.IsNullOrWhiteSpace(candidate) && Directory.Exists(candidate))
            {
                return candidate;
            }
        }

        var candidatePaths = new[]
        {
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Steam"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Steam"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Steam")
        };

        return candidatePaths.FirstOrDefault(Directory.Exists);
    }
}
