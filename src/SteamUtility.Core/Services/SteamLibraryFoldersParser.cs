using SteamUtility.Core.Models;
using SteamUtility.Core.Vdf;

namespace SteamUtility.Core.Services;

public sealed class SteamLibraryFoldersParser
{
    public IReadOnlyList<SteamLibraryFolder> Parse(string vdfContent)
    {
        var root = SimpleVdfReader.Parse(vdfContent);
        var libraryFoldersRoot = root.GetChildren("libraryfolders").FirstOrDefault();

        if (libraryFoldersRoot is null)
        {
            return [];
        }

        var folders = new List<SteamLibraryFolder>();

        foreach (var entry in libraryFoldersRoot.Children)
        {
            foreach (var folderObject in entry.Value)
            {
                var path = folderObject.GetSingleValue("path");
                if (string.IsNullOrWhiteSpace(path))
                {
                    continue;
                }

                var appIds = folderObject
                    .GetChildren("apps")
                    .SelectMany(apps => apps.Values.Keys)
                    .Select(static key => int.TryParse(key, out var appId) ? appId : (int?)null)
                    .Where(static appId => appId.HasValue)
                    .Select(static appId => appId!.Value)
                    .ToArray();

                folders.Add(new SteamLibraryFolder(
                    Key: entry.Key,
                    Path: NormalizePath(path),
                    IsDefault: entry.Key == "0",
                    AppIds: appIds));
            }
        }

        return folders;
    }

    private static string NormalizePath(string path)
        => path.Replace("\\\\", "\\").Replace("\\", "/");
}
