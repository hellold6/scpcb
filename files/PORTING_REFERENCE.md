# SCP:CB Xbox 360 Port — BlitzBasic → C# Translation Reference

## Project Structure

```
repo root/
├── Program.cs                 ← Entry point
├── scpcb.csproj               ← .NET 9 + MonoGame DesktopGL
├── files/                     ← All ported C# (namespace SCPCB360.*)
│   ├── SCPCB360Game.cs        ← Main game loop (Main.bb + Update.bb)
│   ├── GameBootstrap.cs       ← InitNewGame / InitLoadGame
│   ├── GameState.cs           ← Global game state
│   ├── Blitz3D.cs             ← Static B3D.* API (SCPCB360.Engine)
│   ├── BlitzEntity.cs
│   ├── RenderSystem.cs
│   ├── RMeshReader.cs         ← Runtime .rmesh parser
│   ├── PhysicsSystem.cs
│   ├── MapSystem.cs           ← CreateMap, CreateRoom, LoadRoom
│   ├── FillRoomSystem.cs      ← FillRoom() per-room setup
│   ├── RoomTemplateSystem.cs  ← rooms.ini templates
│   ├── EventSystem.cs         ← UpdateEvents port (+ partials)
│   ├── NPCSystem.cs
│   ├── ItemSystem.cs
│   ├── SaveSystem.cs
│   ├── PlayerSystem.cs
│   ├── DoorSystem.cs
│   ├── AudioSystem.cs
│   ├── XInputRouter.cs        ← SCPCB360.Input
│   ├── cook_assets.py         ← OBJ→FBX→XNB pipeline
│   └── PORTING_REFERENCE.md   ← This file
├── Data/                      ← INI configs (copied to build output)
├── GFX/                       ← Textures, RMESH, models (copied to output)
├── SFX/                       ← Audio source
├── Content/Content.mgcb       ← MonoGame content manifest
├── docs/                      ← Port documentation (see docs/README.md)
└── xbox/                      ← Xbox 360 migration kit (excluded from desktop build)
```

See [docs/ARCHITECTURE.md](../docs/ARCHITECTURE.md) for boot flow and system diagrams.

---

## BlitzBasic → C# Idiom Map

### Type declarations → classes

```blitzbasic
; BlitzBasic
Type NPCs
  Field ent, spawnEnt
  Field health#, state
End Type
```

```csharp
// C#
public class NPC {
    public int ent = -1, spawnEnt = -1;
    public float health = 100f;
    public NPCState state = NPCState.Idle;
}
```

### For Each loops → foreach

```blitzbasic
; BlitzBasic
For n.NPCs = Each NPCs
    UpdateNPC(n)
Next
```

```csharp
// C#
foreach (var n in NPCSystem.All)
    UpdateNPC(n, delta);
```

### Global variables → static fields

```blitzbasic
; BlitzBasic
Global player.entity, camPivot, cam
Global health# = 100
Global gamestate = 0
```

```csharp
// C# — in SCPCB360Game or a static GameState class
private int _playerEnt, _camPivot, _cam;
private float _playerHealth = 100f;
private GameState _state = GameState.Playing;
```

---

## Command Mapping Reference

| Blitz3D Command | C# Equivalent |
|---|---|
| `CreatePivot([parent])` | `B3D.CreatePivot([parent])` |
| `LoadMesh(path$, [parent])` | `B3D.LoadMesh(path, [parent])` — cooked `.xnb` via ContentManager |
| `LoadRMesh(file$)` (rooms) | `B3D.LoadRMesh(rmeshPath)` — runtime `.rmesh` via `RMeshReader` |
| `CreateCamera([parent])` | `B3D.CreateCamera([parent])` |
| `CreateLight([type])` | `B3D.CreateLight([type])` |
| `CopyEntity(e, [parent])` | `B3D.CopyEntity(e, [parent])` |
| `PositionEntity e, x, y, z [,global]` | `B3D.PositionEntity(e, x, y, z [,global])` |
| `MoveEntity e, dx, dy, dz` | `B3D.MoveEntity(e, dx, dy, dz)` |
| `RotateEntity e, p, y, r [,global]` | `B3D.RotateEntity(e, p, y, r [,global])` |
| `TurnEntity e, dp, dy, dr` | `B3D.TurnEntity(e, dp, dy, dr)` |
| `ScaleEntity e, sx, sy, sz` | `B3D.ScaleEntity(e, sx, sy, sz)` |
| `PointEntity e, target [,roll]` | `B3D.PointEntity(e, target [,roll])` |
| `AlignToVector e, vx, vy, vz [,axis]` | `B3D.AlignToVector(e, vx, vy, vz [,axis])` |
| `EntityX(e [,global])` | `B3D.EntityX(e [,global])` |
| `EntityY(e [,global])` | `B3D.EntityY(e [,global])` |
| `EntityZ(e [,global])` | `B3D.EntityZ(e [,global])` |
| `EntityPitch(e)` | `B3D.EntityPitch(e)` |
| `EntityYaw(e)` | `B3D.EntityYaw(e)` |
| `EntityRoll(e)` | `B3D.EntityRoll(e)` |
| `EntityDistance(a, b)` | `B3D.EntityDistance(a, b)` |
| `EntityVisible(a, b)` | `B3D.EntityVisible(a, b)` |
| `EntityAlpha e, a` | `B3D.EntityAlpha(e, a)` |
| `EntityColor e, r, g, b` | `B3D.EntityColor(e, r, g, b)` |
| `EntityBlend e, mode` | `B3D.EntityBlend(e, mode)` |
| `HideEntity e` | `B3D.HideEntity(e)` |
| `ShowEntity e` | `B3D.ShowEntity(e)` |
| `FreeEntity e` | `B3D.FreeEntity(e)` |
| `EntityType e, t [,recursive]` | `B3D.EntityType(e, t [,recursive])` |
| `EntityRadius e, r [,y]` | `B3D.EntityRadius(e, r [,y])` |
| `Collisions src, dst, method, resp` | `B3D.Collisions(src, dst, method, resp)` |
| `ATan2(y, x)` | `B3D.ATan2(y, x)` |
| `Sqr(v)` | `B3D.Sqr(v)` |
| `Rand(min, max)` | `B3D.Rand(min, max)` |
| `Rnd(min, max)` | `B3D.Rnd(min, max)` |

### Input mapping (Win32 → XInput)

| CB Win32 Input | XInput Equivalent |
|---|---|
| `MouseXSpeed()` | `XInputRouter.MouseXSpeed()` |
| `MouseYSpeed()` | `XInputRouter.MouseYSpeed()` |
| `MoveMouse(cx, cy)` | *(no-op — thumbstick has no cursor)* |
| `KeyDown(17)` = W forward | `XInputRouter.GetForwardAxis() > 0` |
| `KeyDown(31)` = S backward | `XInputRouter.GetForwardAxis() < 0` |
| `KeyDown(1)` = Escape | `XInputRouter.IsPressed(CBAction.PauseMenu)` |
| `KeyDown(30)` = A strafe | `XInputRouter.GetStrafeAxis() < 0` |
| `KeyDown(32)` = D strafe | `XInputRouter.GetStrafeAxis() > 0` |
| `KeyDown(42)` = Shift sprint | `XInputRouter.IsSprinting()` |
| `KeyHit(56)` = Alt (blink) | `XInputRouter.IsPressed(CBAction.Blink)` |
| `MouseDown(1)` = LMB interact | `XInputRouter.IsHeld(CBAction.Interact)` |

---

## Memory Budget (512 MB Unified)

| Region | Size | Notes |
|---|---|---|
| GFX assets (pre-cached) | ~194.5 MB | All textures + meshes in RAM at boot |
| SFX assets (pre-cached) | ~110.2 MB | All audio in RAM (XMA decoded on demand) |
| Engine + game logic | ~30 MB | C# heap, entity registry, NPC state |
| XNA runtime + OS | ~80 MB | Xbox 360 XNA runtime overhead |
| **Total** | **~415 MB** | **97 MB headroom for runtime allocs** |

Recommendation: Pre-cache all GFX + SFX during the initial loading screen (one large
`Content.Load<>` pass), then never stream assets mid-session. This eliminates CB's
notorious mid-corridor stutter caused by Blitz3D's lazy asset loading.

---

## Xbox 360 Specific Gotchas

1. **Big-Endian vertex buffers**: MGCB with `/platform:Xbox360` handles this automatically
   during the cooking step. Never load raw .obj or .b3d files at runtime on the console.

2. **eDRAM render target**: The 10 MB eDRAM is the implicit render target. Never call
   `GraphicsDevice.GetBackBufferData()` mid-frame — this forces an eDRAM resolve and
   kills framerate. All post-processing must go through a resolve step at frame end.

3. **XMA audio**: The Xenos audio DSP decodes XMA in hardware at zero CPU cost. Use
   SoundEffect (XNB/XMA) for all short effects. Use MediaPlayer for music streams.

4. **Fixed-function vs. shader**: XNA on 360 still goes through the shader pipeline even
   with BasicEffect. BasicEffect on 360 compiles to optimized vertex + pixel shaders.
   For CB's simple vertex-colored, fog-obscured geometry this is perfectly adequate.

5. **No dynamic recompilation**: Unlike emulators, this is native execution. There is no
   JIT warmup penalty after the first draw call per material.
