# Assets

SCP:CB ships with original assets under `GFX/`, `SFX/`, and `Data/`. The MonoGame port uses two loading strategies depending on asset type.

## Loading strategies

| Asset type | Desktop (current) | Console target (Xbox) |
|------------|-------------------|------------------------|
| Room meshes | **Runtime RMESH** (`RMeshReader`) | Pre-baked `.xnb` via MGCB |
| Room textures | **Runtime** from paths in RMESH | Baked DXT `.xnb` |
| Map props (doors, levers) | MGCB `.xnb` when available | Required `.xnb` |
| NPC `.b3d` models | MGCB or future loader | Baked `.xnb` |
| Audio `.ogg` | `Content.Load<SoundEffect>` when cooked | XMA `.xnb` |
| INI data | Copied raw to output | Copied or embedded |

## Room meshes (RMESH)

### Files

- `GFX/map/<name>_opt.rmesh` — optimized room mesh (primary)
- `GFX/map/<name>.rmesh` — fallback
- Referenced from `Data/rooms.ini` as `mesh path=GFX\map\...`

97 `*_opt.rmesh` files cover the room template set.

### Runtime path

```
rooms.ini  →  RoomTemplate.ObjPath
           →  MapSystem.ResolveRMeshPath()
           →  B3D.LoadRMesh(path)
           →  RMeshReader.LoadRenderMesh()
           →  BlitzEntity.RMeshRenderMesh
           →  RenderSystem.DrawRMesh()
```

Collision:

```
RMeshReader.LoadCollisionMesh()  →  RoomInstance.collisionMesh
                                 →  BlitzEntity.CollisionMesh (for physics)
```

Textures referenced inside RMESH are resolved relative to the `.rmesh` directory and loaded with `Texture2D.FromStream` on first draw.

### Scale

Rooms are scaled by `GameState.RoomScale = 8.0 / 2048.0` to match Blitz3D world units.

## MonoGame content pipeline (MGCB)

### Manifest

`Content/Content.mgcb` lists assets for the MonoGame Content Builder (`mgcb`). On desktop dev builds with ARM64 hosts, this file may be **empty** so builds are not blocked by missing assimp native DLLs.

### When you need MGCB

Cook content when you want:

- Door / lever / button `.x` props (`GFX/map/Door01.x`, `leverbase.x`, …)
- NPC `.b3d` / `.x` models as `.xnb`
- SFX as `SoundEffect` `.xnb`
- Xbox 360 deployment (no runtime filesystem parsing)

### Generate Content.mgcb

On an **x64** machine with `mgcb` installed:

```powershell
python files/cook_assets.py --src . --out Content/bin --mgcb --skip-mesh --skip-tex --skip-audio
```

This writes `Content/Content.mgcb` with:

- Map prop entries (`leverbase`, `door01`, `Button`, …)
- One entry per `GFX/map/*_opt.obj` room mesh

Then build:

```powershell
dotnet build scpcb.csproj
```

Cooked `.xnb` files land under `Content/bin/DesktopGL/Content/` and are copied to `bin/Debug/net9.0/Content/`.

### Load paths in code

`B3D.LoadMesh("GFX/map/door01")` normalizes to content key `GFX/map/door01` (no extension). The MGCB `#begin` name must match:

```
#begin GFX/map/door01
/build:../GFX/map/Door01.x
```

## cook_assets.py

Location: `files/cook_assets.py`

Full asset baking pipeline for Xbox / pre-baked desktop builds.

### Workflow

1. **Meshes** — `GFX/map/*.obj` → Blender headless → `.fbx` → MGCB → `.xnb`
2. **Textures** — `GFX/**/*.{jpg,png}` → MGCB with DXT1/DXT5
3. **Audio** — `SFX/**/*.{ogg,wav}` → XMA (or WMA fallback via MGCB)

### Usage

```powershell
python files/cook_assets.py --src C:\path\to\repo --out C:\path\to\cooked_content
```

| Flag | Effect |
|------|--------|
| `--mgcb` | Regenerate `Content/Content.mgcb` only |
| `--platform DesktopGL` | Desktop GL cooks (default) |
| `--platform Xbox360` | Console vertex layout |
| `--skip-mesh` / `--skip-tex` / `--skip-audio` | Skip pipeline stages |
| `--dry` | Print commands without executing |
| `--jobs N` | Parallel workers |

### Dependencies

| Tool | Required for |
|------|------------|
| `mgcb` | All `.xnb` output |
| Blender 4.x + PATH | OBJ → FBX |
| Pillow + numpy | DXT format detection |
| ffmpeg | OGG → WAV for XMA |
| xmaencode (XDK) | Xbox audio (optional; MGCB fallback) |

### Manifest output

`asset_manifest.json` in the output directory lists all discovered meshes, textures, and audio for tooling and CI.

## Map props (shared meshes)

`MapAssets.Initialize()` loads template meshes used by `FillRoomSystem`:

| Handle | Original CB path | Content key |
|--------|------------------|-------------|
| `LeverBaseObj` | `GFX\map\leverbase.x` | `GFX/map/leverbase` |
| `LeverObj` | `GFX\map\leverhandle.x` | `GFX/map/leverhandle` |
| `Monitor` | `GFX\map\monitor.b3d` | `GFX/map/monitor` (`.x` fallback in pipeline) |
| `ButtonObj` | `GFX\map\Button.x` | `GFX/map/Button` |
| `DoorObj` | `GFX\map\door01.x` | `GFX/map/door01` |
| `CamBaseObj` | `GFX\map\cambase.x` | `GFX/map/cambase` |

`DoorSystem.Initialize()` loads `door01` and `doorframe` for `CreateDoor`.

If MGCB has not run, these handles exist but carry null `XnaModel` — props are invisible until content is baked.

## Data files

| File | Purpose |
|------|---------|
| `Data/rooms.ini` | Room template definitions |
| `Data/materials.ini` | Surface material metadata |
| `Data/events.ini` | Event metadata |
| `Data/NPCs.ini` | NPC spawn/config |
| `Data/achievementstrings.ini` | Achievement text and images |
| `Data/SCP-294.ini` | SCP-294 drink definitions |
| `Data/1499chunks.ini` | SCP-1499 dimension chunks |

All are copied to build output via `scpcb.csproj`.

## ARM64 Windows note

MonoGame's MGCB bundles **win-x64 and win-x86 assimp only**. On ARM64 Windows, native model import fails with `BadImageFormatException`. Workarounds:

1. **Recommended for map dev** — rely on RMESH runtime loading (no MGCB entries needed for rooms).
2. **Bake on x64** — run `cook_assets.py` / `dotnet build` on an x64 CI machine; copy `Content/bin` and `bin/.../Content` artifacts back.
3. **Future** — runtime DirectX `.x` parser for props on ARM64.