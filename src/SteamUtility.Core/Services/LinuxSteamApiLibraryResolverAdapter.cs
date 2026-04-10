using SteamUtility.Core.Abstractions;
using SteamUtility.Core.Models;

namespace SteamUtility.Core.Services;

public sealed class LinuxSteamApiLibraryResolverAdapter : ISteamApiLibraryResolver
{
    public string? FindLibraryPath(SteamInstallation installation) => LinuxSteamApiLibraryResolver.FindLibraryPath(installation);
}
