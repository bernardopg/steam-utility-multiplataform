using System.Text.Json;
using SteamUtility.Cli;

namespace SteamUtility.Tests.Cli;

internal static class CommandContractTestHarness
{
    public static CliRunResult Run(string[] args, SteamUtilityCli.CliRuntimeOverrides? overrides = null)
    {
        var stdout = new StringWriter();
        var stderr = new StringWriter();
        var originalOut = Console.Out;
        var originalErr = Console.Error;

        try
        {
            Console.SetOut(stdout);
            Console.SetError(stderr);
            SteamUtilityCli.Run(args, overrides);
        }
        finally
        {
            Console.SetOut(originalOut);
            Console.SetError(originalErr);
        }

        return new CliRunResult(stdout.ToString(), stderr.ToString());
    }

    public static CliRunResult Run(params string[] args)
        => Run(args, null);

    public static JsonDocument ParseJsonFromStdout(CliRunResult result)
    {
        return JsonDocument.Parse(result.Stdout);
    }
}

internal sealed record CliRunResult(string Stdout, string Stderr);
