namespace SteamUtility.Core.Services;

public enum SteamworksInitializationFailure : byte
{
    None = 0,
    LibraryNotFound,
    LibraryLoadFailed,
    ExportNotFound,
    ApiInitFailed,
    UserInterfaceResolutionFailed,
    CurrentStatsRequestFailed,
    GlobalAchievementPercentagesRequestFailed
}

public sealed class SteamworksInitializationException(SteamworksInitializationFailure failureReason, string message)
    : Exception(message)
{
    public SteamworksInitializationFailure FailureReason { get; } = failureReason;
}
