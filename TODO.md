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

## In progress
- [ ] Improve compatibility tool detection heuristics
- [ ] Parse more Steam config/state files beyond compatibility mapping

## Next
- [ ] Parse active user-specific Steam config where relevant
- [ ] Detect tool assignment precedence and edge cases
- [ ] Map compatdata entries back to app names in a richer report
- [ ] Add filtering by AppID/name in CLI
- [ ] Add JSON output mode for automation
- [ ] Add tests for VDF parsing
- [ ] Add tests for library discovery
- [ ] Add tests for app manifest parsing
- [ ] Add tests for compatibility mapping parser
- [ ] Add logging / diagnostics mode
- [ ] Add release build instructions

## Later
- [ ] Evaluate Linux-native Steam API loading strategy
- [ ] Reconstruct original commands one by one behind abstractions
- [ ] Decide what to do with Win32-only idle/window behavior
- [ ] Consider Tauri or other GUI only after core parity is clearer
- [ ] Add CI pipeline
- [ ] Package binaries/releases

## Notes
- The current milestone is discovery/parsing, not full behavioral parity.
- The riskiest missing area is anything coupled to Win32 or Windows-only Steam client loading.
