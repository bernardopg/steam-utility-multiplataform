using SteamUtility.Core.Services;

namespace SteamUtility.Tests;

public static class SteamPlatformRuntimeTests
{
    public static void Current_SelectsPlatformSpecificServices()
    {
        var platform = SteamPlatformRuntime.Current;

        if (OperatingSystem.IsWindows())
        {
            if (platform.Name != "windows") throw new Exception("Expected windows platform name.");
            if (platform.Locator is not WindowsSteamLocator) throw new Exception("Expected Windows locator.");
            if (platform.ClientLibraryLoader is not WindowsSteamClientLibraryLoader) throw new Exception("Expected Windows client loader.");
            if (platform.ApiLibraryResolver is not WindowsSteamApiLibraryResolver) throw new Exception("Expected Windows API resolver.");
            return;
        }

        if (platform.Name != "linux") throw new Exception("Expected linux platform name.");
        if (platform.Locator is not LinuxSteamLocator) throw new Exception("Expected Linux locator.");
        if (platform.ClientLibraryLoader is not LinuxSteamClientLibraryLoader) throw new Exception("Expected Linux client loader.");
        if (platform.ApiLibraryResolver is not LinuxSteamApiLibraryResolverAdapter) throw new Exception("Expected Linux API resolver.");
    }
}
