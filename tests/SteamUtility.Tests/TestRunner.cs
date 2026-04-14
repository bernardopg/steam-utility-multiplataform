namespace SteamUtility.Tests;

public static class TestRunner
{
    public static int Main()
    {
        var tests = new Action[]
        {
            Cli.CliHelpAndDispatchTests.ProgramEntrypoint_ForwardsToSteamUtilityCliRun,
            Cli.CliHelpAndDispatchTests.Run_WithoutArgs_PrintsUsage,
            Cli.CliHelpAndDispatchTests.Run_WithHelpFlag_PrintsUsage,
            Cli.CliHelpAndDispatchTests.Run_WithShortHelpFlag_PrintsUsage,
            Cli.CliHelpAndDispatchTests.Run_WithUnknownCommand_PrintsUsage,
            Cli.CliHelpAndDispatchTests.Run_WithCheckOwnershipAlias_PrintsSameUsage,
            Cli.CheckOwnershipCliTests.Run_WithoutOutputPath_PrintsUsage,
            Cli.CheckOwnershipCliTests.Run_WithMissingInstallation_ReturnsFailureJson,
            Cli.CheckOwnershipCliTests.Run_WithInvalidAppIdPayload_ReturnsFailureJson,
            Cli.CheckOwnershipCliTests.Run_WithFakeOwnedApps_WritesUpstreamCompatibleGamesJson,
            Cli.CheckOwnershipCliTests.Run_WhenOwnershipServiceFails_ReturnsFailureReason,
            Cli.IdleCliTests.Run_WithInvalidAppId_ReturnsLegacyJsonError,
            Cli.IdleCliTests.Run_WithMissingInstallation_ReturnsLegacyJsonError,
            Cli.IdleCliTests.Run_WithIdleOverride_ReturnsSuccessPayload,
            Cli.IdleCliTests.Run_WithOptionalAppName_PreservesNameInPayload,
            Cli.IdleCliTests.Run_WhenSteamworksInitFails_ReturnsFailureReason,
            SteamAppManifestParserTests.Parse_AppManifest_ReturnsExpectedValues,
            SteamLibraryFoldersParserTests.Parse_MultipleLibraryFolders_ReturnsExpectedEntries,
            SteamLibraryScannerTests.ScanInstalledApps_FindsManifestsAcrossLibraries,
            SteamCompatDataScannerTests.Scan_FindsCompatDataAndPfx,
            SteamCompatibilityToolScannerTests.Scan_FindsCustomAndBundledCompatibilityTools,
            SteamConfigCompatibilityParserTests.Parse_CompatToolMapping_ReturnsAssignments,
            SteamLoginUsersParserTests.Parse_LoginUsers_ReturnsMostRecentUserFirst,
            SteamUserConfigScannerTests.Scan_FindsLocalAndSharedConfigs,
            SteamCompatibilityReportServiceTests.Build_WithMissingFiles_ReturnsEmptyReport,
            SteamCompatibilityReportServiceTests.Build_MergesAppCompatDataAndMappings,
            SteamStateReportServiceTests.Build_SummarizesSteamStateFiles,
            SteamPlatformRuntimeTests.Current_SelectsPlatformSpecificServices,
            LinuxSteamClientLibraryTests.FindLibraryPath_Prefers64BitClient_WhenMultipleCandidatesExist,
            LinuxSteamClientLibraryTests.FindLibraryPath_ReturnsNull_WhenClientLibraryDoesNotExist,
            LinuxSteamApiLibraryResolverTests.FindLibraryPath_PrefersLocalLibraryFromSteamGameFolders,
            LinuxSteamApiLibraryResolverTests.FindLibraryPath_UsesEnvironmentOverrideWhenPresent
        };

        foreach (var test in tests)
        {
            try
            {
                test();
                Console.WriteLine($"PASS {test.Method.DeclaringType?.Name}.{test.Method.Name}");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"FAIL {test.Method.DeclaringType?.Name}.{test.Method.Name}: {ex.Message}");
                return 1;
            }
        }

        return 0;
    }
}
