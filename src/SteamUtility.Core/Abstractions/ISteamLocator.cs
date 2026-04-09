namespace SteamUtility.Core.Abstractions;

public interface ISteamLocator
{
    string? TryGetSteamRoot();
}
