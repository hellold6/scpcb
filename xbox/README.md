# SCP:CB Xbox 360 Migration Kit

This folder is a preparation kit, not a verified Xbox 360 build.

No fake XNA references or placeholder assemblies are included. If the local legacy XNA 4.0 Xbox 360 toolchain is missing, the build script reports that as a blocker.

## Contents

- `SCPCB360.Xna360.csproj` - attempted legacy XNA 4.0 Xbox 360 project file.
- `src/` - copied current C# source from the desktop port.
- `build-desktop-audit.ps1` - verifies the current desktop project and writes `logs/desktop-build.txt`.
- `build-xna360.ps1` - checks for real XNA 4.0 Game Studio MSBuild targets, then attempts an Xbox 360 build only if they exist.
- `DEPENDENCY_AUDIT.md` - dependency and source compatibility audit.
- `logs/` - build/audit output.

## Completed

- Current C# source copied into `xbox/src`.
- Attempted XNA 4.0 Xbox 360 project file created.
- Desktop verification script created.
- XNA build script created with honest tooling checks.
- Dependency audit written.
- Root desktop project excludes `xbox/**/*.cs` so migration-copy source files do not get compiled into the current DesktopGL build.

## Partially Completed

- The attempted XNA project is a migration artifact. It has not been proven to compile for Xbox 360 on this machine.
- The current source still contains desktop/debug paths that need conditional compilation or offline asset baking before a real Xbox compile can succeed.

## Blockers

- Current main project targets `net9.0` and `MonoGame.Framework.DesktopGL`.
- DesktopGL uses SDL/OpenGL/OpenAL-Soft and cannot execute on Xbox 360.
- Xbox 360 XNA requires legacy XNA Game Studio 4.0 build/deploy tooling.
- Runtime filesystem texture loading and RMESH parsing must become prebuilt Xbox-compatible content.
- Mouse/keyboard fallback and console I/O must be removed or excluded for Xbox.
- Original Windows native DLLs from the Blitz3D project cannot be used on Xbox 360.

## How To Run Audits

From the repository root:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\xbox\build-desktop-audit.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File .\xbox\build-xna360.ps1
```

Expected current result:

- Desktop audit should build the current Windows/Desktop project.
- XNA build may fail early with a tooling blocker if `Microsoft.Xna.GameStudio.targets` is not installed.
- If script execution policy has already been relaxed locally, direct `.\xbox\*.ps1` invocation is also fine.

## What Still Prevents Real Xbox 360 Execution

The project must be converted from DesktopGL/.NET 9 runtime behavior to an Xbox 360-compatible XNA 4.0 or native homebrew build. That requires real legacy tooling, Xbox-compatible cooked assets, controller-only input, and removal of runtime raw-file texture/model/audio loading.
