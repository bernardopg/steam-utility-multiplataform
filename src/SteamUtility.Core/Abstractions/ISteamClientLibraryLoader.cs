using SteamUtility.Core.Interop;

namespace SteamUtility.Core.Abstractions;

public interface ISteamClientLibraryLoader
{
    string? FindLibraryPath(string steamRoot);

    bool TryLoad(string steamRoot);

    TInterface CreateInterface<TInterface>(string versionString)
        where TInterface : INativeWrapper, new();
}
