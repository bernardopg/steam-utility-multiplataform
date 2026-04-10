# Proton and CompatData Discovery

## Purpose
Map Linux compatibility data that is relevant for Windows game execution through Proton.

## What is scanned
### Compatibility tools
- `compatibilitytools.d/` under the Steam root for custom tools
- `steamapps/common/` under the Steam root for bundled Proton/runtime installs

### Per-game compatibility data
- `steamapps/compatdata/<AppId>/`
- `steamapps/compatdata/<AppId>/pfx`
- `config/config.vdf` for explicit compatibility assignments

## Why this matters
This is the Linux-side replacement for assumptions that a Windows-native runtime environment exists automatically.

## Current implementation status
Implemented:
- Discovery of custom compatibility tool roots
- Heuristic discovery of bundled Proton/runtime folders
- Discovery of `compatdata` entries by AppID
- Parsing of explicit compatibility-tool mappings from Steam config
- Exposure path ready for CLI commands

Not implemented yet:
- Parse all assignment edge cases
- Inspect prefixes in detail
- Launch or emulate original Windows-only features
