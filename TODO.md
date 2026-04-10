# TODO

## Done
- [x] Create public repository scaffold
- [x] Move solution to .NET 8
- [x] Create `SteamUtility.Core`
- [x] Create `SteamUtility.Cli`
- [x] Add Linux Steam root locator
- [x] Add minimal VDF parser
- [x] Parse `libraryfolders.vdf`
- [x] Model Steam installation and library folders
- [x] Parse `appmanifest_*.acf`
- [x] Scan installed apps across library folders
- [x] Add CLI commands for `detect`, `libraries`, and `apps`
- [x] Scan `compatdata/<AppId>` folders
- [x] Scan bundled/custom compatibility tools
- [x] Add CLI commands for `compatdata` and `compat-tools`
- [x] Write `README.md`
- [x] Write `TODO.md`
- [x] Parse `config/config.vdf` compatibility mappings
- [x] Add merged compatibility report command
- [x] Add CLI filtering by AppID/text
- [x] Add JSON output mode
- [x] Add initial test project
- [x] Add Linux `steamclient.so` loader scaffold
- [x] Add initial `check_ownership` port through native Steam client interfaces
- [x] Port achievement/stat commands through native Linux `libsteam_api.so`
- [x] Improve compatibility tool detection heuristics
- [x] Expand automated tests
- [x] Parse more Steam config/state files beyond compatibility mapping
- [x] Broaden runtime validation of state-changing achievement/stat commands
- [x] Add explicit `--help` and `-h` handling in the CLI
- [x] Restore the optional app name argument for `idle <app_id> [app_name]`
- [x] Replace the Win32 hidden idle window with a Linux keep-alive loop
- [x] Align native initialization failures with upstream-style failure reasons
- [x] Match `get_achievement_data` callback and timeout semantics more closely
- [x] Align command success and error messages with upstream behavior where practical
- [x] Parse active user-specific Steam config where relevant
- [x] Detect tool assignment precedence and edge cases
- [x] Map compatdata entries back to app names in a richer report
- [x] Add JSON schema stability notes / versioning
- [x] Add tests for compatibility tool scanner
- [x] Add tests for compatdata scanner
- [x] Add logging / diagnostics mode
- [x] Add release build instructions
- [x] Add CI pipeline
- [x] Package binaries/releases

## Base parity
- [x] Add a platform selector that chooses Linux or Windows services at runtime
- [x] Add Windows Steam root discovery
- [x] Add Windows `steamclient.dll` loading scaffold
- [x] Add Windows Steamworks native library resolution
- [x] Add Windows release artifacts to the publishing workflow
- [x] Update README and release notes for cross-platform support

## Later
- [ ] Consider Tauri or other GUI only after core parity is clearer

## Notes
- The repository now covers Linux and Windows platform selection, Linux discovery, and the upstream ownership/achievement/stat command surface.
- The remaining work is now strategic follow-up in `Later`, mainly whether the project should ever add a GUI layer.
