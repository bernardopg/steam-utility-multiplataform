using System.Text.Json;
using SteamUtility.Cli;
using SteamUtility.Core.Services;
using SteamUtility.Tests.Fakes;

namespace SteamUtility.Tests.Cli;

public static class AchievementMutationCliTests
{
    public static void Run_SingleMutation_WithMissingAchievementId_ReturnsRequiredError()
    {
        var result = CommandContractTestHarness.Run(
            ["unlock_achievement", "440"],
            new SteamUtilityCli.CliRuntimeOverrides
            {
                ResolveInstallation = () => FakeSteamInstallationFactory.Create()
            });

        using var payload = JsonDocument.Parse(result.Stdout);
        if (payload.RootElement.GetProperty("error").GetString() != "achievement_id is required")
        {
            throw new Exception("Expected missing achievement_id error.");
        }
    }

    public static void Run_UnlockAchievement_Success_ReturnsSuccessMessage()
    {
        var result = CommandContractTestHarness.Run(
            ["unlock_achievement", "440", "ACH_WIN_ONE_GAME"],
            new SteamUtilityCli.CliRuntimeOverrides
            {
                ResolveInstallation = () => FakeSteamInstallationFactory.Create(),
                RunSingleAchievementMutation = (_, _, _, shouldUnlock) => new SteamUtilityCli.AchievementMutationCommandResult(
                    true,
                    SuccessMessage: shouldUnlock ? "Successfully unlocked achievement" : "Successfully locked achievement")
            });

        using var payload = JsonDocument.Parse(result.Stdout);
        if (payload.RootElement.GetProperty("success").GetString() != "Successfully unlocked achievement")
        {
            throw new Exception("Expected unlock success message.");
        }
    }

    public static void Run_LockAchievement_Success_ReturnsSuccessMessage()
    {
        var result = CommandContractTestHarness.Run(
            ["lock_achievement", "440", "ACH_WIN_ONE_GAME"],
            new SteamUtilityCli.CliRuntimeOverrides
            {
                ResolveInstallation = () => FakeSteamInstallationFactory.Create(),
                RunSingleAchievementMutation = (_, _, _, shouldUnlock) => new SteamUtilityCli.AchievementMutationCommandResult(
                    true,
                    SuccessMessage: shouldUnlock ? "Successfully unlocked achievement" : "Successfully locked achievement")
            });

        using var payload = JsonDocument.Parse(result.Stdout);
        if (payload.RootElement.GetProperty("success").GetString() != "Successfully locked achievement")
        {
            throw new Exception("Expected lock success message.");
        }
    }

    public static void Run_SingleMutation_WithMissingAchievement_ReturnsFailureMessage()
    {
        var result = CommandContractTestHarness.Run(
            ["unlock_achievement", "440", "MISSING"],
            new SteamUtilityCli.CliRuntimeOverrides
            {
                ResolveInstallation = () => FakeSteamInstallationFactory.Create(),
                RunSingleAchievementMutation = (_, _, _, _) => new SteamUtilityCli.AchievementMutationCommandResult(
                    false,
                    Error: "Failed to get achievement data. The achievement might not exist")
            });

        using var payload = JsonDocument.Parse(result.Stdout);
        if (payload.RootElement.GetProperty("error").GetString() != "Failed to get achievement data. The achievement might not exist")
        {
            throw new Exception("Expected missing-achievement failure message.");
        }
    }

    public static void Run_SingleMutation_WhenSteamworksInitFails_ReturnsFailureReason()
    {
        var result = CommandContractTestHarness.Run(
            ["unlock_achievement", "440", "ACH_WIN_ONE_GAME"],
            new SteamUtilityCli.CliRuntimeOverrides
            {
                ResolveInstallation = () => FakeSteamInstallationFactory.Create(),
                RunSingleAchievementMutation = (_, _, _, _) => throw new SteamworksInitializationException(
                    SteamworksInitializationFailure.ApiInitFailed,
                    "Failed to initialize Steam API. Make sure Steam is running and the selected app id is valid.")
            });

        using var payload = JsonDocument.Parse(result.Stdout);
        if (payload.RootElement.GetProperty("failureReason").GetString() != SteamworksInitializationFailure.ApiInitFailed.ToString())
        {
            throw new Exception("Expected ApiInitFailed failure reason.");
        }
    }

    public static void Run_ToggleAchievement_UnlockPath_ReturnsUnlockSuccessMessage()
    {
        var result = CommandContractTestHarness.Run(
            ["toggle_achievement", "440", "ACH_WIN_ONE_GAME"],
            new SteamUtilityCli.CliRuntimeOverrides
            {
                ResolveInstallation = () => FakeSteamInstallationFactory.Create(),
                RunToggleAchievement = (_, _, _) => new SteamUtilityCli.AchievementMutationCommandResult(
                    true,
                    SuccessMessage: "Successfully unlocked achievement")
            });

        using var payload = JsonDocument.Parse(result.Stdout);
        if (payload.RootElement.GetProperty("success").GetString() != "Successfully unlocked achievement")
        {
            throw new Exception("Expected toggle unlock success message.");
        }
    }

    public static void Run_ToggleAchievement_LockPath_ReturnsLockSuccessMessage()
    {
        var result = CommandContractTestHarness.Run(
            ["toggle_achievement", "440", "ACH_WIN_ONE_GAME"],
            new SteamUtilityCli.CliRuntimeOverrides
            {
                ResolveInstallation = () => FakeSteamInstallationFactory.Create(),
                RunToggleAchievement = (_, _, _) => new SteamUtilityCli.AchievementMutationCommandResult(
                    true,
                    SuccessMessage: "Successfully locked achievement")
            });

        using var payload = JsonDocument.Parse(result.Stdout);
        if (payload.RootElement.GetProperty("success").GetString() != "Successfully locked achievement")
        {
            throw new Exception("Expected toggle lock success message.");
        }
    }

    public static void Run_ToggleAchievement_ValidationFailure_ReturnsError()
    {
        var result = CommandContractTestHarness.Run(
            ["toggle_achievement", "440", "ACH_WIN_ONE_GAME"],
            new SteamUtilityCli.CliRuntimeOverrides
            {
                ResolveInstallation = () => FakeSteamInstallationFactory.Create(),
                RunToggleAchievement = (_, _, _) => new SteamUtilityCli.AchievementMutationCommandResult(
                    false,
                    Error: "Achievement state validation failed after storing changes")
            });

        using var payload = JsonDocument.Parse(result.Stdout);
        if (payload.RootElement.GetProperty("error").GetString() != "Achievement state validation failed after storing changes")
        {
            throw new Exception("Expected toggle validation failure message.");
        }
    }

    public static void Run_UnlockAllAchievements_Success_ReturnsSuccessMessage()
    {
        var result = CommandContractTestHarness.Run(
            ["unlock_all_achievements", "440"],
            new SteamUtilityCli.CliRuntimeOverrides
            {
                ResolveInstallation = () => FakeSteamInstallationFactory.Create(),
                RunToggleAllAchievements = (_, _, shouldUnlock) => new SteamUtilityCli.AchievementMutationCommandResult(
                    true,
                    SuccessMessage: shouldUnlock ? "Successfully unlocked all achievements" : "Successfully locked all achievements")
            });

        using var payload = JsonDocument.Parse(result.Stdout);
        if (payload.RootElement.GetProperty("success").GetString() != "Successfully unlocked all achievements")
        {
            throw new Exception("Expected unlock-all success message.");
        }
    }

    public static void Run_UnlockAllAchievements_PartialFailure_ReturnsError()
    {
        var result = CommandContractTestHarness.Run(
            ["unlock_all_achievements", "440"],
            new SteamUtilityCli.CliRuntimeOverrides
            {
                ResolveInstallation = () => FakeSteamInstallationFactory.Create(),
                RunToggleAllAchievements = (_, _, _) => new SteamUtilityCli.AchievementMutationCommandResult(
                    false,
                    Error: "One or more achievements failed to unlock")
            });

        using var payload = JsonDocument.Parse(result.Stdout);
        if (payload.RootElement.GetProperty("error").GetString() != "One or more achievements failed to unlock")
        {
            throw new Exception("Expected unlock-all partial failure message.");
        }
    }

    public static void Run_LockAllAchievements_Success_ReturnsSuccessMessage()
    {
        var result = CommandContractTestHarness.Run(
            ["lock_all_achievements", "440"],
            new SteamUtilityCli.CliRuntimeOverrides
            {
                ResolveInstallation = () => FakeSteamInstallationFactory.Create(),
                RunToggleAllAchievements = (_, _, shouldUnlock) => new SteamUtilityCli.AchievementMutationCommandResult(
                    true,
                    SuccessMessage: shouldUnlock ? "Successfully unlocked all achievements" : "Successfully locked all achievements")
            });

        using var payload = JsonDocument.Parse(result.Stdout);
        if (payload.RootElement.GetProperty("success").GetString() != "Successfully locked all achievements")
        {
            throw new Exception("Expected lock-all success message.");
        }
    }

    public static void Run_LockAllAchievements_PostResetValidationFailure_ReturnsError()
    {
        var result = CommandContractTestHarness.Run(
            ["lock_all_achievements", "440"],
            new SteamUtilityCli.CliRuntimeOverrides
            {
                ResolveInstallation = () => FakeSteamInstallationFactory.Create(),
                RunToggleAllAchievements = (_, _, _) => new SteamUtilityCli.AchievementMutationCommandResult(
                    false,
                    Error: "Post-reset validation failed")
            });

        using var payload = JsonDocument.Parse(result.Stdout);
        if (payload.RootElement.GetProperty("error").GetString() != "Post-reset validation failed")
        {
            throw new Exception("Expected post-reset validation failure message.");
        }
    }
}
