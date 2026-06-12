# SCP - Containment Breach

The game is based on the works of the SCP Foundation community (http://www.scp-wiki.net/).

This game and the source code are licensed under Creative Commons Attribution-ShareAlike 3.0 License.

http://creativecommons.org/licenses/by-sa/3.0/

## Original game (Blitz3D)

Requirements:

- Blitz3D v1.108

Beware — the source code is perhaps more horrifying than the game itself!

Build and run the classic version with Blitz3D using `Main.bb` and the original `GFX/`, `SFX/`, and `Data/` trees in this repository.

## MonoGame port (SCPCB360)

This repository also contains an in-progress **C# / MonoGame / .NET 9** port of the game, targeting desktop first with an experimental Xbox 360 migration kit under `xbox/`.

### Quick start

```powershell
dotnet build scpcb.csproj
dotnet run --project scpcb.csproj
```

### Documentation

Full port documentation lives in **[docs/](docs/README.md)**:

| Guide | Description |
|-------|-------------|
| [docs/BUILD.md](docs/BUILD.md) | Build, run, platform notes |
| [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md) | Systems, boot flow, file map |
| [docs/ASSETS.md](docs/ASSETS.md) | RMESH loading, MGCB, `cook_assets.py` |
| [docs/PORT_STATUS.md](docs/PORT_STATUS.md) | What's ported vs. remaining |
| [files/PORTING_REFERENCE.md](files/PORTING_REFERENCE.md) | Blitz3D → C# command reference |

Xbox 360 migration notes: [xbox/README.md](xbox/README.md)