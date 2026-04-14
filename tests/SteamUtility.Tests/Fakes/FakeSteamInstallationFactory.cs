using SteamUtility.Core.Models;

namespace SteamUtility.Tests.Fakes;

internal static class FakeSteamInstallationFactory
{
    public static SteamInstallation Create(
        string rootPath = "/tmp/fake-steam",
        string steamAppsPath = "/tmp/fake-steam/steamapps")
    {
        return new SteamInstallation(rootPath, steamAppsPath, []);
    }
}
