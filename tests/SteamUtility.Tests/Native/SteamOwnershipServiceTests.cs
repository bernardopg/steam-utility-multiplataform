using SteamUtility.Core.Models;
using SteamUtility.Core.Services;
using SteamUtility.Tests.Fakes;

namespace SteamUtility.Tests.Native;

public static class SteamOwnershipServiceTests
{
    public static void GetOwnedApps_DeduplicatesAndUsesFallbackNames()
    {
        using var loader = new FakeSteamClientLibraryLoader(
            ownedAppIds: [730, 570],
            appNames: new Dictionary<uint, string?>
            {
                [730] = "Counter-Strike 2",
                [570] = null
            });

        var installation = FakeSteamInstallationFactory.Create("/tmp/fake-steam", "/tmp/fake-steam/steamapps");
        var service = new SteamOwnershipService(() => new SteamClientConnection(loader));

        var ownedApps = service.GetOwnedApps(installation, [730, 570, 730, 999]);

        if (ownedApps.Count != 2)
        {
            throw new Exception("Expected duplicate app ids to be removed.");
        }

        if (ownedApps[0] != new SteamOwnedApp(730, "Counter-Strike 2"))
        {
            throw new Exception("Unexpected first owned app.");
        }

        if (ownedApps[1] != new SteamOwnedApp(570, "App 570"))
        {
            throw new Exception("Unexpected fallback app name.");
        }

        if (loader.TryLoadCalls != 1)
        {
            throw new Exception("Expected the client library to load once.");
        }

        if (loader.CreateInterfaceCalls != 1)
        {
            throw new Exception("Expected one Steam client interface creation.");
        }

        if (loader.CreateSteamPipeCalls != 1)
        {
            throw new Exception("Expected one Steam pipe creation.");
        }

        if (loader.ConnectToGlobalUserCalls != 1)
        {
            throw new Exception("Expected one Steam user connection.");
        }

        if (loader.ReleaseUserCalls != 1)
        {
            throw new Exception("Expected Steam user release on dispose.");
        }

        if (loader.ReleaseSteamPipeCalls != 1)
        {
            throw new Exception("Expected Steam pipe release on dispose.");
        }
    }
}
