# SCPCB360 Documentation

This folder documents the **MonoGame / .NET 9 desktop port** of SCP: Containment Breach (originally Blitz3D + BlitzBasic). The original game files (`*.bb`, `GFX/`, `SFX/`, `Data/`) remain in the repository root.

## Guides

| Document | Contents |
|----------|----------|
| [BUILD.md](BUILD.md) | Prerequisites, compile, run, platform notes (x64 vs ARM64) |
| [ARCHITECTURE.md](ARCHITECTURE.md) | Code layout, namespaces, boot flow, system map |
| [ASSETS.md](ASSETS.md) | RMESH runtime loading, MGCB pipeline, `cook_assets.py` |
| [PORT_STATUS.md](PORT_STATUS.md) | BlitzBasic → C# coverage, gaps, roadmap |

## Related docs (elsewhere in repo)

| Path | Contents |
|------|----------|
| [files/PORTING_REFERENCE.md](../files/PORTING_REFERENCE.md) | Blitz3D command → C# mapping, input table, Xbox memory budget |
| [xbox/README.md](../xbox/README.md) | Xbox 360 migration kit (legacy XNA tooling, not a verified console build) |
| [xbox/DEPENDENCY_AUDIT.md](../xbox/DEPENDENCY_AUDIT.md) | Dependency and compatibility audit for console target |

## Quick start

```powershell
dotnet build scpcb.csproj
dotnet run --project scpcb.csproj
```

`Data/` and `GFX/` are copied into the build output automatically. Room geometry loads from `GFX/map/*_opt.rmesh` at runtime — no content bake is required for map meshes on desktop.