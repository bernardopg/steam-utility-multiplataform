using SteamUtility.Core.Abstractions;

namespace SteamUtility.Core.Services;

public sealed record SteamPlatformContext(
    string Name,
    ISteamLocator Locator,
    ISteamClientLibraryLoader ClientLibraryLoader,
    ISteamApiLibraryResolver ApiLibraryResolver);
