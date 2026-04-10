# Steam Library Discovery (Linux + Windows)

## Purpose
Document how the project discovers Steam installations, library folders, and app manifests across Linux and Windows.

## Discovery flow
1. Resolve the Steam root directory for the current OS.
2. Resolve the default `steamapps` path under the root.
3. Parse `steamapps/libraryfolders.vdf`.
4. Normalize and validate discovered library folders.
5. Scan `steamapps/appmanifest_*.acf` in each library.

## Steam root resolution by platform
### Linux
- `~/.steam/steam`
- `~/.local/share/Steam`

### Windows
- Registry candidates:
	- `HKEY_LOCAL_MACHINE\Software\Valve\Steam` (`InstallPath`)
	- `HKEY_LOCAL_MACHINE\Software\WOW6432Node\Valve\Steam` (`InstallPath`)
	- `HKEY_CURRENT_USER\Software\Valve\Steam` (`SteamPath` / `InstallPath`)
- Fallback directories:
	- `%ProgramFiles(x86)%\Steam`
	- `%ProgramFiles%\Steam`
	- `%LocalAppData%\Steam`

## Files scanned
- `steamapps/libraryfolders.vdf`
- `steamapps/appmanifest_*.acf`
- `config/config.vdf`
- `config/loginusers.vdf`
- `userdata/*/config/localconfig.vdf`
- `userdata/*/config/sharedconfig.vdf`

## Current status
Implemented:
- Linux and Windows root discovery
- Minimal VDF parser and Valve-keyvalue tree handling
- `libraryfolders.vdf` parsing and normalization
- Installed app manifest parsing
- Compatibility/report scanners that reuse library discovery
- Active Steam user and per-user config scans

Notes:
- Proton-related data (`compatdata`, compatibility tools) is usually Linux-relevant, but command surfaces remain available cross-platform.
- The project currently targets Linux and Windows; macOS runtime selection is not implemented.
