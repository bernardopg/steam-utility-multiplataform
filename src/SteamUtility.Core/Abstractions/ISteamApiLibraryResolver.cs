using SteamUtility.Core.Models;

namespace SteamUtility.Core.Abstractions;

public interface ISteamApiLibraryResolver
{
    string? FindLibraryPath(SteamInstallation installation);
}
