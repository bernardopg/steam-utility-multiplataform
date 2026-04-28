# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commits

Never add `Co-Authored-By: Claude` or any Claude/Anthropic co-author trailer to commit messages.

## Commands

```bash
dotnet restore
dotnet build steam-utility-multiplataform.sln -c Release

# Tests — use dotnet run, NOT dotnet test (custom runner)
dotnet run --project tests/SteamUtility.Tests -c Release
```

Publish a self-contained single-file binary (what the SGI release pipeline produces):
```bash
dotnet publish src/SteamUtility.Cli \
  -c Release \
  -r linux-x64 \
  --self-contained true \
  -p:PublishSingleFile=true \
  -o ./artifacts/linux-x64
```

Run a discovery command without building a full release:
```bash
dotnet run --project src/SteamUtility.Cli -- detect
dotnet run --project src/SteamUtility.Cli -- state-report --json
```

## Architecture

### Project layout

```
steam-utility-multiplataform.sln
├── src/
│   ├── SteamUtility.Core/     # all library code
│   └── SteamUtility.Cli/      # CLI entrypoint and command dispatch
└── tests/
    └── SteamUtility.Tests/    # custom test runner
```

`Directory.Build.props` sets the shared target framework (`net10.0`), nullable, and implicit usings for all projects.

### SteamUtility.Core

The library is organized into:

| Directory | Contents |
|---|---|
| `Abstractions/` | `ISteamLocator`, `ISteamClientLibraryLoader`, `ISteamApiLibraryResolver` — the three platform interfaces |
| `Services/` | All service implementations (see below) |
| `Models/` | Data classes (`SteamInstallation`, `SteamLibraryFolder`, `SteamApp`, etc.) |
| `Vdf/` | Minimal VDF key-value parser used for `.acf` and `.vdf` files |
| `Interop/` | Native P/Invoke wrappers and Steamworks interface structs |

**Platform selection** — `SteamPlatformRuntime.Current` (in `Services/`) reads `RuntimeInformation.IsOSPlatform` and returns a `SteamPlatformContext` that wires up the correct implementations of the three platform interfaces. All callers go through this context; nothing instantiates platform-specific services directly.

**Linux services:** `LinuxSteamLocator`, `LinuxSteamClientLibraryLoader`, `LinuxSteamClientLibrary`, `LinuxSteamApiLibraryResolver`

**Windows services:** `WindowsSteamLocator`, `WindowsSteamClientLibraryLoader`, `WindowsSteamApiLibraryResolver`

**Shared services:** `SteamInstallationService`, `SteamLibraryScanner`, `SteamAppManifestParser`, `SteamLibraryFoldersParser`, `SteamCompatDataScanner`, `SteamCompatibilityToolScanner`, `SteamConfigCompatibilityParser`, `SteamLoginUsersParser`, `SteamUserConfigScanner`, `SteamCompatibilityReportService`, `SteamStateReportService`, `SteamOwnershipService`, `SteamworksSession`, `SteamApiNative`, `StatsSchemaLoader`

### SteamUtility.Cli

`SteamUtilityCli.cs` contains the full command dispatch as a `switch` on `options.Command`. `Program.cs` is a one-liner that calls `SteamUtilityCli.Run(args)`.

Adding a new command: add a `case` in `SteamUtilityCli.cs` and a `PrintXxx` method in the same file. Commands accept underscore and hyphen variants where relevant.

### Tests

`tests/SteamUtility.Tests/` contains a custom runner (`TestRunner.cs`). All tests are registered there and executed via:
```bash
dotnet run --project tests/SteamUtility.Tests -c Release
```
`dotnet test` will not find tests — do not use it.

Test files follow the naming pattern `<ServiceName>Tests.cs`.

### Command groups

**Discovery — no live Steam required:**
`detect`, `libraries`, `apps`, `compatdata`, `compat-tools`, `compat-mapping`, `compat-report`, `state-report`

**Steam-native — requires a running Steam session:**
`check_ownership`, `idle`, `get_achievement_data`, `unlock_achievement`, `lock_achievement`, `toggle_achievement`, `unlock_all_achievements`, `lock_all_achievements`, `update_stats`, `reset_all_stats`

Steam-native commands use `SteamworksSession` to initialize the Steamworks SDK, then call the relevant service. The session init loads the platform-appropriate `steamclient` shared library resolved through the `ISteamClientLibraryLoader` abstraction.

### JSON output stability

Discovery/report commands (`apps`, `libraries`, `compatdata`, etc.) have a versioned output contract (schema v1). Steam-native commands intentionally preserve upstream-compatible legacy output shapes and are not yet versioned. See `docs/JSON_OUTPUTS.md` for details.

### Known gaps

- Windows-specific services (`WindowsSteamLocator`, `WindowsSteamClientLibraryLoader`, `WindowsSteamApiLibraryResolver`) have no dedicated automated tests.
- Live Windows validation (real Steam session) has not been executed — only Linux has been live-validated.

## CI (`.github/workflows/ci.yml`)

Builds and runs tests on `ubuntu-latest`, `windows-latest`, and `macos-latest`. Generates a coverage report via Coverlet and comments it on PRs. Pinned versions: `actions/checkout@v6.0.2`, `actions/setup-dotnet@v5.2.0`, `actions/github-script@v9`.
