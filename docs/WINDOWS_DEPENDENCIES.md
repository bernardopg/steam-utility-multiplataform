# Windows Platform Dependencies (Current State)

## Purpose
Document the Windows-specific integrations that are intentionally present in the current cross-platform architecture.

## Windows-specific components in use
### Steam installation discovery
- Registry lookup for Steam root:
	- `HKEY_LOCAL_MACHINE\Software\Valve\Steam` (`InstallPath`)
	- `HKEY_LOCAL_MACHINE\Software\WOW6432Node\Valve\Steam` (`InstallPath`)
	- `HKEY_CURRENT_USER\Software\Valve\Steam` (`SteamPath` / `InstallPath`)
- Fallback filesystem paths under Program Files and Local AppData.

### Steam client loading
- Native loader for `steamclient.dll`.
- `SetDllDirectory` usage to align DLL search behavior with Steam install layout.
- `CreateInterface` export binding for native interface wrappers.

### Steamworks library resolution
- Candidate resolution for `steam_api64.dll` and `steam_api.dll` in common game layouts.
- PE header architecture checks to avoid loading mismatched binaries.

## Isolation strategy
- Windows-specific behavior is kept behind interfaces (`ISteamLocator`, `ISteamClientLibraryLoader`, `ISteamApiLibraryResolver`).
- Runtime selection chooses Windows or Linux implementations automatically.
- Command-level behavior remains shared at the CLI layer.

## Parity expectations
- Discovery/report commands should behave similarly on Linux and Windows.
- Proton/compatdata data is generally sparse on native Windows installations and may legitimately return empty sets.
