# Windows Real Steam Validation Checklist

Use this checklist on a real Windows machine with Steam installed and logged in.

## Purpose

The automated test suite proves deterministic CLI behavior, but it does not prove the Windows native Steam integration against a live Steam installation. This checklist documents the manual validation path for:

- `WindowsSteamLocator`
- `WindowsSteamClientLibraryLoader`
- `WindowsSteamApiLibraryResolver`
- the native Steam command flow used by the upstream-compatible CLI commands

## Preconditions

Required:
- Windows 10 or Windows 11 x64
- Steam installed and logged in
- `.NET 10 SDK`
- this repository checked out locally
- at least one known game/app that exposes Steam user stats or achievements
- PowerShell

Recommended before starting:
```powershell
cd C:\path\to\steam-utility-multiplataform
dotnet build steam-utility-multiplataform.sln
dotnet run --project tests\SteamUtility.Tests
```

## Windows platform service validation

### 1. Confirm Steam registry discovery inputs

Inspect the registry keys used by `WindowsSteamLocator`:
```powershell
Get-ItemProperty -Path 'HKLM:\Software\Valve\Steam' -ErrorAction SilentlyContinue
Get-ItemProperty -Path 'HKLM:\Software\WOW6432Node\Valve\Steam' -ErrorAction SilentlyContinue
Get-ItemProperty -Path 'HKCU:\Software\Valve\Steam' -ErrorAction SilentlyContinue
```

Expected:
- at least one key exists
- `InstallPath`, `SteamPath`, or both point to the active Steam installation

### 2. Validate `WindowsSteamLocator`

```powershell
dotnet run --project src\SteamUtility.Cli -- detect
dotnet run --project src\SteamUtility.Cli -- detect --json
```

Expected:
- human output reports a valid Steam installation path
- JSON output has `found: true`
- JSON output has a non-empty `rootPath`
- the reported root contains `steam.exe`

### 3. Validate `WindowsSteamClientLibraryLoader`

Confirm `steamclient.dll` exists under the detected Steam root:
```powershell
$steamRoot = (dotnet run --project src\SteamUtility.Cli -- detect --json | ConvertFrom-Json).rootPath
Get-ChildItem -Path $steamRoot -Filter steamclient.dll -Recurse -ErrorAction SilentlyContinue | Select-Object -First 20 FullName
```

Then run an ownership command that requires the Steam client native loader:
```powershell
dotnet run --project src\SteamUtility.Cli -- check_ownership "$env:TEMP\steam-utility-games.json" "[440,570,730]" --json --diagnostics
```

Expected:
- no `DllNotFoundException`
- no architecture mismatch error
- no `CreateInterface` binding failure
- failure, if any, is a Steam session/account failure rather than a loader failure
- with Steam running and logged in, the command returns `success: true`

### 4. Validate `WindowsSteamApiLibraryResolver`

Find Steamworks API DLL candidates in installed app folders:
```powershell
$steamRoot = (dotnet run --project src\SteamUtility.Cli -- detect --json | ConvertFrom-Json).rootPath
Get-ChildItem -Path $steamRoot -Filter steam_api64.dll -Recurse -ErrorAction SilentlyContinue | Select-Object -First 20 FullName
Get-ChildItem -Path $steamRoot -Filter steam_api.dll -Recurse -ErrorAction SilentlyContinue | Select-Object -First 20 FullName
```

Then run an achievement/stat read command against a known app:
```powershell
dotnet run --project src\SteamUtility.Cli -- get_achievement_data <APP_ID> "$env:TEMP\steam-utility-cache" --json --diagnostics
```

Expected:
- resolver selects a DLL matching the current process architecture
- no bad image format / architecture mismatch error
- no `DllNotFoundException`
- command writes cache data under `%TEMP%\steam-utility-cache`
- output includes achievement/stat data for a game that exposes it

## Baseline discovery commands

### 5. Library folders
```powershell
dotnet run --project src\SteamUtility.Cli -- libraries
dotnet run --project src\SteamUtility.Cli -- libraries --json
```

Expected:
- at least one library folder
- default Steam library is marked or discoverable

### 6. Installed apps
```powershell
dotnet run --project src\SteamUtility.Cli -- apps
dotnet run --project src\SteamUtility.Cli -- apps --json
```

Expected:
- installed manifests listed
- app IDs and names match the Steam library

### 7. State report
```powershell
dotnet run --project src\SteamUtility.Cli -- state-report --json --diagnostics > "$env:TEMP\state-report.json" 2> "$env:TEMP\state-report.stderr"
Get-Content "$env:TEMP\state-report.stderr"
Get-Content "$env:TEMP\state-report.json" | ConvertFrom-Json
```

Expected:
- stderr contains diagnostics
- stdout remains valid JSON
- active account and installation summary are coherent

## Native Steam command flow validation

Choose a safe test title/account before running mutation commands. Prefer a throwaway or low-impact game with known achievements and stats.

### 8. Ownership
```powershell
dotnet run --project src\SteamUtility.Cli -- check_ownership "$env:TEMP\games.json" "[<APP_ID>]" --json
Get-Content "$env:TEMP\games.json" | ConvertFrom-Json
```

Expected:
- `success: true`
- `ownedCount >= 1` when the account owns the app
- output file contains `games`

### 9. Idle
```powershell
dotnet run --project src\SteamUtility.Cli -- idle <APP_ID> "Windows validation test"
```

Expected:
- Steam reports the app as running/idling
- process stays alive until interrupted
- stopping the command cleans up without orphan processes

### 10. Achievement/stat export
```powershell
dotnet run --project src\SteamUtility.Cli -- get_achievement_data <APP_ID> "$env:TEMP\steam-utility-cache" --json
```

Expected:
- cache file written under `%TEMP%\steam-utility-cache\<steamid>\achievement_data\<APP_ID>.json`
- file contains `achievements` and `stats` arrays

### 11. Specific item lookup
```powershell
dotnet run --project src\SteamUtility.Cli -- get_achievement_data <APP_ID> "$env:TEMP\steam-utility-cache" <ITEM_ID> --json
```

Expected:
- payload describes one achievement or stat
- unknown item returns `{"error":"ID not found"}`

### 12. Achievement mutations
```powershell
dotnet run --project src\SteamUtility.Cli -- unlock_achievement <APP_ID> <ACH_ID> --json
dotnet run --project src\SteamUtility.Cli -- lock_achievement <APP_ID> <ACH_ID> --json
dotnet run --project src\SteamUtility.Cli -- toggle_achievement <APP_ID> <ACH_ID> --json
dotnet run --project src\SteamUtility.Cli -- unlock_all_achievements <APP_ID> --json
dotnet run --project src\SteamUtility.Cli -- lock_all_achievements <APP_ID> --json
```

Expected:
- each command returns a success payload
- follow-up `get_achievement_data` confirms the expected state
- final state is restored to the original observed state

### 13. Stat mutations
```powershell
dotnet run --project src\SteamUtility.Cli -- update_stats <APP_ID> "[{\"name\":\"STAT_NAME\",\"value\":1}]" --json
dotnet run --project src\SteamUtility.Cli -- reset_all_stats <APP_ID> --json
```

Expected:
- update command returns success
- follow-up `get_achievement_data` confirms the changed stat
- reset command returns success
- final stat value is restored or documented

## Failure paths

Run these after baseline success:

- close Steam completely and rerun `check_ownership`
- use an app ID the account does not own
- use an unknown achievement/stat item ID
- run with `--diagnostics` and confirm stdout JSON remains parseable

Expected:
- failures are structured and explain whether the issue is installation discovery, native library loading, Steam user connection, or game data
- diagnostics do not corrupt stdout JSON

## Record results

For each run, record:
- date
- Windows version/build
- CPU architecture
- .NET SDK version
- Steam account identifier, if safe to record
- Steam install path
- app IDs tested
- selected achievement/stat IDs
- command result
- stdout/stderr for failures
- final restored game state after mutations

## Exit criteria

Windows platform validation is complete when:
- `detect` proves `WindowsSteamLocator` against the real machine
- `check_ownership` proves `WindowsSteamClientLibraryLoader` against a live Steam session
- `get_achievement_data` proves `WindowsSteamApiLibraryResolver` against a real app's Steamworks DLL
- every upstream native command has been exercised at least once
- mutation commands are verified through follow-up reads
- failures are either fixed or added to `TODO.md`
