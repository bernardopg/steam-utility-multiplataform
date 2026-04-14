using System.Diagnostics;

namespace SteamUtility.Tests.Cli;

public static class CliHelpAndDispatchTests
{
    public static void ProgramEntrypoint_ForwardsToSteamUtilityCliRun()
    {
        var repoRoot = FindRepositoryRoot();
        var startInfo = new ProcessStartInfo("dotnet", "run --project src/SteamUtility.Cli -- --help")
        {
            WorkingDirectory = repoRoot,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        using var process = Process.Start(startInfo) ?? throw new Exception("Failed to start CLI process.");
        var stdout = process.StandardOutput.ReadToEnd();
        var stderr = process.StandardError.ReadToEnd();
        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            throw new Exception($"Expected CLI process to exit successfully. stderr={stderr}");
        }

        if (!stdout.Contains("steam-utility-multiplataform", StringComparison.Ordinal) ||
            !stdout.Contains("Usage:", StringComparison.Ordinal))
        {
            throw new Exception("Expected top-level Program.cs entrypoint to print CLI usage.");
        }
    }

    public static void Run_WithoutArgs_PrintsUsage()
    {
        var result = CommandContractTestHarness.Run();

        if (!result.Stdout.Contains("Usage:", StringComparison.Ordinal))
        {
            throw new Exception("Expected usage output when no args are provided.");
        }
    }

    public static void Run_WithHelpFlag_PrintsUsage()
    {
        var result = CommandContractTestHarness.Run("--help");

        if (!result.Stdout.Contains("Usage:", StringComparison.Ordinal))
        {
            throw new Exception("Expected usage output for --help.");
        }
    }

    public static void Run_WithShortHelpFlag_PrintsUsage()
    {
        var result = CommandContractTestHarness.Run("-h");

        if (!result.Stdout.Contains("Usage:", StringComparison.Ordinal))
        {
            throw new Exception("Expected usage output for -h.");
        }
    }

    public static void Run_WithUnknownCommand_PrintsUsage()
    {
        var result = CommandContractTestHarness.Run("definitely-unknown-command");

        if (!result.Stdout.Contains("Usage:", StringComparison.Ordinal))
        {
            throw new Exception("Expected usage output for unknown command.");
        }
    }

    public static void Run_WithCheckOwnershipAlias_PrintsSameUsage()
    {
        var underscore = CommandContractTestHarness.Run("check_ownership");
        var hyphen = CommandContractTestHarness.Run("check-ownership");

        if (underscore.Stdout != hyphen.Stdout)
        {
            throw new Exception("Expected identical output for check_ownership and check-ownership without args.");
        }
    }

    private static string FindRepositoryRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current is not null)
        {
            if (Directory.Exists(Path.Combine(current.FullName, "src", "SteamUtility.Cli")) &&
                Directory.Exists(Path.Combine(current.FullName, "tests", "SteamUtility.Tests")))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new Exception("Could not locate repository root from test base directory.");
    }
}
