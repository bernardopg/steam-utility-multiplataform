# Cross-Platform Status and Roadmap

## Objective
Maintain a single, testable CLI codebase that runs on Linux and Windows, while keeping platform-specific behavior isolated behind abstractions.

## Porting status
The original porting effort is complete for Linux + Windows runtime support.

Delivered:
1. Migration to .NET 8 with shared core + CLI projects.
2. Platform runtime selector for Linux and Windows services.
3. Steam installation/library discovery and app/compat scanning.
4. Native Steam client loading for ownership and Steamworks command flows.
5. Cross-platform CI/release packaging for Linux and Windows artifacts.

## Current architecture principles
- Keep platform-specific concerns in dedicated service implementations.
- Keep command behavior consistent between platforms whenever possible.
- Keep filesystem/config scanning independent from native library loading.
- Keep JSON discovery/report outputs schema-versioned.

## Remaining roadmap
1. Improve compatibility-tool/runtime heuristics for edge-case Steam layouts.
2. Expand integration-style tests around native client initialization failures.
3. Continue upstream command-behavior alignment where practical.
4. Reassess GUI layering only after CLI surface remains stable.

## Known scope boundaries
- Official runtime support currently targets Linux and Windows.
- Proton/compatdata-centric insights are primarily meaningful on Linux environments.
- Steamworks mutation commands require a running and logged-in Steam client.

## Ongoing success criteria
- `detect`, `libraries`, `apps`, and `state-report` return coherent results on Linux and Windows hosts with Steam installed.
- Ownership and Steamworks commands surface clear failure reasons when native initialization cannot be completed.
