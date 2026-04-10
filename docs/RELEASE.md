# Release Build Instructions

## Local build
```bash
dotnet test tests/SteamUtility.Tests
dotnet publish src/SteamUtility.Cli -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true
dotnet publish src/SteamUtility.Cli -c Release -r linux-arm64 --self-contained true -p:PublishSingleFile=true
dotnet publish src/SteamUtility.Cli -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

## Suggested output paths
- Published binaries land under `src/SteamUtility.Cli/bin/Release/<target-framework>/<runtime>/publish/`
- Release packaging should produce one zip archive per runtime.

## Runtime matrix
- `linux-x64`
- `linux-arm64`
- `win-x64`

## Release workflow
- Push a tag that matches `v*`
- GitHub Actions builds the CLI for `linux-x64`, `linux-arm64`, and `win-x64`
- The workflow uploads zip assets to the GitHub release

## Notes
- The project is cross-platform, with Linux and Windows release assets built from the same CLI codebase.
- If the CLI command surface changes, update `README.md`, `docs/JSON_OUTPUTS.md`, and any command-specific docs together.
- Keep docs aligned with runtime behavior whenever platform-specific wording changes.
