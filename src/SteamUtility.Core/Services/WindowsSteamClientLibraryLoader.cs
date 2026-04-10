using System.Runtime.InteropServices;
using SteamUtility.Core.Abstractions;
using SteamUtility.Core.Interop;
using System.Runtime.Versioning;

namespace SteamUtility.Core.Services;

[SupportedOSPlatform("windows")]
public sealed class WindowsSteamClientLibraryLoader : ISteamClientLibraryLoader
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    private delegate IntPtr CreateInterfaceDelegate(string versionString, IntPtr returnCode);

    private static readonly string[] CandidateRelativePaths =
    [
        "steamclient.dll",
        Path.Combine("bin", "steamclient.dll")
    ];

    private static nint _libraryHandle;
    private static CreateInterfaceDelegate? _createInterface;

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetDllDirectory(string lpPathName);

    public string? FindLibraryPath(string steamRoot)
    {
        return CandidateRelativePaths
            .Select(relativePath => Path.Combine(steamRoot, relativePath))
            .FirstOrDefault(File.Exists);
    }

    public TInterface CreateInterface<TInterface>(string versionString)
        where TInterface : INativeWrapper, new()
    {
        if (_createInterface is null)
        {
            throw new InvalidOperationException("Steam client library has not been loaded.");
        }

        var interfaceAddress = _createInterface(versionString, IntPtr.Zero);
        if (interfaceAddress == IntPtr.Zero)
        {
            throw new InvalidOperationException($"Failed to create native Steam interface '{versionString}'.");
        }

        var wrapper = new TInterface();
        wrapper.Initialize(interfaceAddress);
        return wrapper;
    }

    public bool TryLoad(string steamRoot)
    {
        if (_libraryHandle != IntPtr.Zero && _createInterface is not null)
        {
            return true;
        }

        var libraryPath = FindLibraryPath(steamRoot);
        if (string.IsNullOrWhiteSpace(libraryPath))
        {
            return false;
        }

        var dllSearchPath = Path.Combine(steamRoot, "bin");
        if (!Directory.Exists(dllSearchPath))
        {
            dllSearchPath = steamRoot;
        }

        SetDllDirectory(dllSearchPath);

        if (!NativeLibrary.TryLoad(libraryPath, out _libraryHandle))
        {
            _libraryHandle = IntPtr.Zero;
            _createInterface = null;
            return false;
        }

        if (!NativeLibrary.TryGetExport(_libraryHandle, "CreateInterface", out var export))
        {
            NativeLibrary.Free(_libraryHandle);
            _libraryHandle = IntPtr.Zero;
            _createInterface = null;
            return false;
        }

        _createInterface = Marshal.GetDelegateForFunctionPointer<CreateInterfaceDelegate>(export);
        return true;
    }
}
