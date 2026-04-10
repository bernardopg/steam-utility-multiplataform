using SteamUtility.Core.Abstractions;
using SteamUtility.Core.Interop;

namespace SteamUtility.Core.Services;

public sealed class LinuxSteamClientLibraryLoader : ISteamClientLibraryLoader
{
    public string? FindLibraryPath(string steamRoot) => LinuxSteamClientLibrary.FindLibraryPath(steamRoot);

    public bool TryLoad(string steamRoot) => LinuxSteamClientLibrary.TryLoad(steamRoot);

    public TInterface CreateInterface<TInterface>(string versionString)
        where TInterface : INativeWrapper, new()
    {
        return LinuxSteamClientLibrary.CreateInterface<TInterface>(versionString);
    }
}
