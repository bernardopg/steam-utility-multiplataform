using System.Text.Json;
using SteamUtility.Cli;
using SteamUtility.Core.Services;
using SteamUtility.Tests.Fakes;

namespace SteamUtility.Tests.Cli;

public static class IdleCliTests
{
    public static void Run_WithInvalidAppId_ReturnsLegacyJsonError()
    {
        var result = CommandContractTestHarness.Run(
            ["idle", "not-a-number"],
            new SteamUtilityCli.CliRuntimeOverrides
            {
                ResolveInstallation = () => FakeSteamInstallationFactory.Create()
            });

        using var payload = JsonDocument.Parse(result.Stdout);
        if (payload.RootElement.GetProperty("error").GetString() != "Invalid app_id")
        {
            throw new Exception("Expected Invalid app_id error.");
        }
    }

    public static void Run_WithMissingInstallation_ReturnsLegacyJsonError()
    {
        var result = CommandContractTestHarness.Run(
            ["idle", "440"],
            new SteamUtilityCli.CliRuntimeOverrides
            {
                ResolveInstallation = () => null
            });

        using var payload = JsonDocument.Parse(result.Stdout);
        if (payload.RootElement.GetProperty("error").GetString() != "Steam installation not found.")
        {
            throw new Exception("Expected missing-installation error.");
        }
    }

    public static void Run_WithIdleOverride_ReturnsSuccessPayload()
    {
        var result = CommandContractTestHarness.Run(
            ["idle", "440"],
            new SteamUtilityCli.CliRuntimeOverrides
            {
                ResolveInstallation = () => FakeSteamInstallationFactory.Create(),
                RunIdle = (_, _, _) => { }
            });

        using var payload = JsonDocument.Parse(result.Stdout);
        var root = payload.RootElement;
        if (root.GetProperty("success").GetString() != "Steam API initialized") throw new Exception("Expected success message.");
        if (root.GetProperty("appId").GetUInt32() != 440) throw new Exception("Expected appId 440.");
        if (root.GetProperty("appName").GetString() != "Idling") throw new Exception("Expected default app name.");
    }

    public static void Run_WithOptionalAppName_PreservesNameInPayload()
    {
        var result = CommandContractTestHarness.Run(
            ["idle", "440", "Team Fortress 2"],
            new SteamUtilityCli.CliRuntimeOverrides
            {
                ResolveInstallation = () => FakeSteamInstallationFactory.Create(),
                RunIdle = (_, _, _) => { }
            });

        using var payload = JsonDocument.Parse(result.Stdout);
        if (payload.RootElement.GetProperty("appName").GetString() != "Team Fortress 2")
        {
            throw new Exception("Expected custom app name in success payload.");
        }
    }

    public static void Run_WhenSteamworksInitFails_ReturnsFailureReason()
    {
        var result = CommandContractTestHarness.Run(
            ["idle", "440"],
            new SteamUtilityCli.CliRuntimeOverrides
            {
                ResolveInstallation = () => FakeSteamInstallationFactory.Create(),
                RunIdle = (_, _, _) => throw new SteamworksInitializationException(
                    SteamworksInitializationFailure.ApiInitFailed,
                    "Failed to initialize Steam API. Make sure Steam is running and the selected app id is valid.")
            });

        using var payload = JsonDocument.Parse(result.Stdout);
        var root = payload.RootElement;
        if (root.GetProperty("failureReason").GetString() != SteamworksInitializationFailure.ApiInitFailed.ToString())
        {
            throw new Exception("Expected ApiInitFailed failure reason.");
        }
    }
}
