using SteamUtility.Core.Abstractions;
using SteamUtility.Core.Models;
using SteamUtility.Core.Services;

ISteamLocator locator = new LinuxSteamLocator();
var installationService = new SteamInstallationService(locator);
var installation = installationService.TryResolve();

if (args.Length == 0)
{
    PrintUsage();
    return;
}

switch (args[0].ToLowerInvariant())
{
    case "detect":
        Console.WriteLine(installation is null
            ? "Steam installation not found."
            : $"Steam installation found at: {installation.RootPath}");
        return;

    case "libraries":
        PrintLibraries(installation);
        return;

    case "apps":
        PrintApps(installation);
        return;

    case "compatdata":
        PrintCompatData(installation);
        return;

    case "compat-tools":
        PrintCompatibilityTools(installation);
        return;

    default:
        PrintUsage();
        return;
}

static void PrintLibraries(SteamInstallation? installation)
{
    if (installation is null)
    {
        Console.WriteLine("Steam installation not found.");
        return;
    }

    Console.WriteLine($"Steam root: {installation.RootPath}");
    Console.WriteLine("Library folders:");

    foreach (var library in installation.LibraryFolders)
    {
        var marker = library.IsDefault ? "*" : "-";
        Console.WriteLine($"  {marker} [{library.Key}] {library.Path}");
    }
}

static void PrintApps(SteamInstallation? installation)
{
    if (installation is null)
    {
        Console.WriteLine("Steam installation not found.");
        return;
    }

    var scanner = new SteamLibraryScanner();
    var apps = scanner.ScanInstalledApps(installation);

    if (apps.Count == 0)
    {
        Console.WriteLine("No installed Steam app manifests were found.");
        return;
    }

    Console.WriteLine($"Detected {apps.Count} installed Steam app(s):");

    foreach (var app in apps)
    {
        Console.WriteLine($"  - {app.AppId}: {app.Name} [{app.InstallDirectory}]");
    }
}

static void PrintCompatData(SteamInstallation? installation)
{
    if (installation is null)
    {
        Console.WriteLine("Steam installation not found.");
        return;
    }

    var scanner = new SteamCompatDataScanner();
    var entries = scanner.Scan(installation);

    if (entries.Count == 0)
    {
        Console.WriteLine("No compatdata entries were found.");
        return;
    }

    Console.WriteLine($"Detected {entries.Count} compatdata entr{(entries.Count == 1 ? "y" : "ies")}:");

    foreach (var entry in entries)
    {
        Console.WriteLine($"  - AppId {entry.AppId}: {entry.CompatDataPath}");
    }
}

static void PrintCompatibilityTools(SteamInstallation? installation)
{
    if (installation is null)
    {
        Console.WriteLine("Steam installation not found.");
        return;
    }

    var scanner = new SteamCompatibilityToolScanner();
    var tools = scanner.Scan(installation);

    if (tools.Count == 0)
    {
        Console.WriteLine("No compatibility tools were found.");
        return;
    }

    Console.WriteLine($"Detected {tools.Count} compatibility tool(s):");

    foreach (var tool in tools)
    {
        var kind = tool.IsCustom ? "custom" : "bundled";
        Console.WriteLine($"  - {tool.Name} ({kind}) -> {tool.RootPath}");
    }
}

static void PrintUsage()
{
    Console.WriteLine("steam-utility-linux bootstrap");
    Console.WriteLine("Usage:");
    Console.WriteLine("  detect         Detect the local Steam installation path on Linux");
    Console.WriteLine("  libraries      List discovered Steam library folders");
    Console.WriteLine("  apps           List installed Steam apps from appmanifest files");
    Console.WriteLine("  compatdata     List per-app compatdata directories");
    Console.WriteLine("  compat-tools   List bundled and custom compatibility tools");
}
