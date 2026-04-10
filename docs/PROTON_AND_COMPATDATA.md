# Proton and CompatData Discovery

## Purpose
Map Steam compatibility data for Proton-based execution and explain cross-platform command behavior.

This domain is primarily Linux-focused, but the command surface is still available on Windows.

## What is scanned
### Compatibility tools
- `compatibilitytools.d/` under the Steam root for custom tools
- `steamapps/common/` under the Steam root for bundled Proton/runtime installs

### Per-game compatibility data
- `steamapps/compatdata/<AppId>/`
- `steamapps/compatdata/<AppId>/pfx`
- `config/config.vdf` for explicit compatibility assignments

## Why this matters
On Linux, this explains which compatibility runtime a title is expected to use and where its prefix data lives.

On Windows, these locations often do not exist and empty results are expected.

## Assignment precedence
- Explicit `config/config.vdf` mappings win over inferred tool guesses.
- Custom tools from `compatibilitytools.d/` are preferred over bundled tools under `steamapps/common/` when the same normalized tool name exists in both places.
- Tool name matching is normalized to ignore punctuation and spacing differences so common Proton/runtime naming variants still resolve.

## Current implementation status
Implemented:
- Discovery of custom compatibility tool roots
- Heuristic discovery of bundled Proton/runtime folders
- Discovery of `compatdata` entries by AppID
- Parsing of explicit compatibility-tool mappings from Steam config
- Resolved tool details in the compatibility report
- CLI support via `compatdata`, `compat-tools`, `compat-mapping`, and `compat-report`

Not implemented yet:
- Deep inspection of prefix internals beyond path-level discovery
- Runtime launch orchestration/emulation behavior
