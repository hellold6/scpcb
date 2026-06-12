# Build and Run

## Requirements

| Tool | Version | Purpose |
|------|---------|---------|
| [.NET SDK](https://dotnet.microsoft.com/download) | 9.0+ | Compile and run |
| MonoGame 3.8 | via NuGet | `MonoGame.Framework.DesktopGL`, `MonoGame.Content.Builder.Task` |
| Windows / Linux / macOS | — | DesktopGL target |

Optional (asset cooking only):

| Tool | Purpose |
|------|---------|
| `mgcb` (dotnet tool) | Bake `.x`/`.fbx`/textures/audio → `.xnb` |
| Blender 4.x | OBJ → FBX in `cook_assets.py` |
| Python 3.11+ | Run `files/cook_assets.py` |

## Build

From the repository root:

```powershell
dotnet restore scpcb.csproj
dotnet build scpcb.csproj
```

Output: `bin/Debug/net9.0/scpcb.exe` (or `scpcb.dll` on non-Windows).

### What gets copied to output

`scpcb.csproj` copies these trees next to the executable:

- `Data/` — `rooms.ini`, NPC config, events, achievements, etc.
- `GFX/` — textures, `.rmesh` room meshes, NPC models, UI art

MonoGame content (`.xnb`) is optional for **room** rendering because rooms use runtime RMESH loading. Prop meshes (doors, levers, buttons) still expect cooked content when MGCB is used — see [ASSETS.md](ASSETS.md).

## Run

```powershell
dotnet run --project scpcb.csproj
```

Or launch `bin/Debug/net9.0/scpcb.exe` directly. Working directory must be the output folder (dotnet run handles this).

### Controls

Desktop builds use `XInputRouter`, which maps keyboard/mouse to Xbox-style actions:

| Action | Default binding |
|--------|-----------------|
| Move | WASD |
| Look | Mouse |
| Blink | Left Alt |
| Interact | Left mouse |
| Sprint | Shift |
| Pause | Escape |

Full mapping is in [files/PORTING_REFERENCE.md](../files/PORTING_REFERENCE.md).

## Platform notes

### x64 Windows / Linux / macOS

Normal build. To bake prop meshes and audio into `.xnb`:

```powershell
python files/cook_assets.py --src . --out Content/bin --mgcb --skip-mesh --skip-tex --skip-audio
dotnet build scpcb.csproj
```

Use `--platform DesktopGL` (default) for desktop; use `Xbox360` when targeting console cooks.

### ARM64 Windows (e.g. Snapdragon laptops)

The project **builds and runs** on ARM64. Room meshes load from `.rmesh` without MGCB.

**MGCB / assimp limitation:** MonoGame's content builder only ships x64/x86 `assimp.dll`. On ARM64 Windows, `mgcb` cannot import `.x` or `.fbx` models. `Content/Content.mgcb` is intentionally empty so builds are not blocked. Map props (doors, levers) will not appear until:

- you bake content on an x64 machine, or
- a runtime `.x` loader is added.

### Xbox 360

See [xbox/README.md](../xbox/README.md). The desktop port does not target Xbox directly; the `xbox/` folder is a separate migration artifact.

## Troubleshooting

| Symptom | Likely cause | Fix |
|---------|--------------|-----|
| Black screen, no rooms | `GFX/map/*.rmesh` missing from output | Rebuild; confirm `GFX/` copy in `.csproj` |
| `rooms.ini` not found | `Data/` not beside executable | Same as above |
| MGCB `assimp.dll` / `BadImageFormatException` | ARM64 or wrong DLL arch | Use empty `Content.mgcb` for dev; bake on x64 |
| Crash on start | Unhandled exception | Run via `dotnet run` — `Program.cs` prints stack trace |
| No door/lever models | No `.xnb` props cooked | Bake on x64 or implement runtime loader |

## Project layout (build-relevant)

```
scpcb.csproj          # .NET 9 + MonoGame; copies Data/ and GFX/
Program.cs            # Entry point → SCPCB360Game
files/                # All ported C# systems (compiled by SDK glob)
Content/
  Content.mgcb        # MonoGame content manifest (may be empty on ARM64)
Data/                 # Game INI data
GFX/                  # Source art and RMESH room files
SFX/                  # OGG/WAV audio (runtime load when cooked)
xbox/                 # Excluded from desktop build (Compile Remove)
```