using System.Runtime.InteropServices;
using SteamUtility.Core.Abstractions;
using SteamUtility.Core.Models;
using System.Runtime.Versioning;

namespace SteamUtility.Core.Services;

[SupportedOSPlatform("windows")]
public sealed class WindowsSteamApiLibraryResolver : ISteamApiLibraryResolver
{
    private static readonly string[] CandidateRelativePaths =
    [
        Path.Combine("bin", "win64", "steam_api64.dll"),
        Path.Combine("bin", "steam_api64.dll"),
        "steam_api64.dll",
        Path.Combine("bin", "steam_api.dll"),
        "steam_api.dll"
    ];

    public string? FindLibraryPath(SteamInstallation installation)
    {
        var envOverride = Environment.GetEnvironmentVariable("STEAMWORKS_NATIVE_LIBRARY");
        if (!string.IsNullOrWhiteSpace(envOverride) &&
            File.Exists(envOverride) &&
            MatchesCurrentProcessArchitecture(envOverride))
        {
            return envOverride;
        }

        var gameDirectories = installation.LibraryFolders
            .Select(library => Path.Combine(library.Path, "steamapps", "common"))
            .Where(Directory.Exists)
            .SelectMany(commonPath => Directory.EnumerateDirectories(commonPath))
            .ToArray();

        foreach (var relativePath in CandidateRelativePaths)
        {
            foreach (var gameDirectory in gameDirectories)
            {
                var candidate = Path.Combine(gameDirectory, relativePath);
                if (File.Exists(candidate) && MatchesCurrentProcessArchitecture(candidate))
                {
                    return candidate;
                }
            }
        }

        return null;
    }

    private static bool MatchesCurrentProcessArchitecture(string libraryPath)
    {
        try
        {
            using var stream = File.OpenRead(libraryPath);
            using var reader = new BinaryReader(stream);

            if (reader.ReadUInt16() != 0x5A4D)
            {
                return false;
            }

            stream.Position = 0x3C;
            var peHeaderOffset = reader.ReadInt32();
            stream.Position = peHeaderOffset;

            if (reader.ReadUInt32() != 0x00004550)
            {
                return false;
            }

            var machine = reader.ReadUInt16();

            return RuntimeInformation.ProcessArchitecture switch
            {
                Architecture.X64 => machine == 0x8664,
                Architecture.X86 => machine == 0x014c,
                Architecture.Arm64 => machine == 0xAA64,
                _ => true
            };
        }
        catch
        {
            return false;
        }
    }
}
