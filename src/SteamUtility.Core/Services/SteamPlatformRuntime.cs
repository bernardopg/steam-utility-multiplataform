namespace SteamUtility.Core.Services;

public static class SteamPlatformRuntime
{
    public static SteamPlatformContext Current { get; } = SteamPlatformContextFactory.CreateCurrent();
}

internal static class SteamPlatformContextFactory
{
    public static SteamPlatformContext CreateCurrent()
    {
        if (OperatingSystem.IsWindows())
        {
            return new SteamPlatformContext(
                "windows",
                new WindowsSteamLocator(),
                new WindowsSteamClientLibraryLoader(),
                new WindowsSteamApiLibraryResolver());
        }

        return new SteamPlatformContext(
            "linux",
            new LinuxSteamLocator(),
            new LinuxSteamClientLibraryLoader(),
            new LinuxSteamApiLibraryResolverAdapter());
    }
}
