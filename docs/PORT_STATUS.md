# Port Status

Snapshot of the BlitzBasic → C# MonoGame port. Line counts are approximate; run `agent-tools/parity_audit.py` to refresh.

## Summary

| Area | Status |
|------|--------|
| Engine (`B3D`, render, RMESH) | **Functional** — rooms draw from `.rmesh` |
| Map generation (`CreateMap`) | **Ported** — grid pipeline + special rooms |
| FillRoom (per-room setup) | **Ported** — 79 C# room cases vs 97 BB `Case` blocks |
| Events (`UpdateEvents`) | **Mostly ported** — 80 handlers, 28 updater methods |
| Save / load | **Ported** — binary format; rooms recreated via `CreateRoom` |
| NPCs | **Partial** — 21 types defined, 13 AI updaters |
| Items | **Partial** — system exists; template registry incomplete |
| Audio | **Partial** — door/interact SFX; content bake pending |
| Map props (doors/levers) | **Blocked on ARM64** — need MGCB on x64 |
| Xbox 360 | **Not runnable** — see `xbox/README.md` |

## Coverage by source file

| BlitzBasic | BB lines | C# lines | C# files |
|------------|----------|----------|----------|
| Main.bb | 12,087 | 1,767 | GameBootstrap, GameState, Player, Door, Gui, B3D |
| MapSystem.bb | 8,751 | 5,893 | MapSystem, FillRoom, RoomTemplate, MapAssets, Forest, Waypoint |
| UpdateEvents.bb | 10,182 | 1,308 | EventSystem (+ partials) |
| NPCs.bb | 7,461 | 755 | NPCSystem |
| Save.bb | 2,538 | 836 | SaveSystem |
| Menu.bb | 2,637 | 919 | MenuSystem, TextRenderer |
| Items.bb | 887 | 504 | ItemSystem |
| LoadAllSounds.bb | 221 | 214 | AudioSystem |

Event cases: **93 BB** → **80 C#** handlers.  
FillRoom cases: **97 BB** → **79 C#** `case` branches.  
Item templates: **133 BB** `CreateItemTemplate` → registry still being filled.

## What works today (desktop)

- Build and launch via .NET 9 + MonoGame DesktopGL
- Main menu and new-game bootstrap
- Procedural map generation from `rooms.ini` templates
- Room mesh rendering from `*_opt.rmesh` with textures
- RMESH collision mesh attached to room entities
- FillRoom spawns doors, objects, and room-specific entities (logic ported; some props invisible without XNB)
- Event system init and update loop
- Player movement, mouse look, blink/stamina HUD skeleton
- Save/load deserializes player, map grid, rooms, doors, NPCs, events

## Known gaps

### Rendering and assets

- [ ] MGCB prop meshes on ARM64 (doors, levers, buttons, monitors)
- [ ] NPC `.b3d` model loading in gameplay
- [ ] Full texture/material system from `materials.ini`
- [ ] Light cones / volumetric room lighting
- [ ] SCP-860 forest generation (`ForestSystem` stub)

### Gameplay

- [ ] Complete NPC AI for all 21 types (13 updaters today)
- [ ] All item templates and use handlers
- [ ] Full SCP encounter behavior parity (106 pocket dimension, 173 intro polish, etc.)
- [ ] Maintenance tunnel generation
- [ ] Waypoint pathfinding integration

### Audio

- [ ] Cook full `SFX/` tree to `.xnb`
- [ ] 3D positional audio parity with FMOD
- [ ] Music streaming edge cases

### Platform

- [ ] Xbox 360 XNA build with pre-baked assets only
- [ ] Remove runtime filesystem texture loads for console
- [ ] Controller-only input path (no mouse fallback)

## Recommended next steps

1. **x64 CI bake** — run `cook_assets.py --mgcb` + full mesh/audio cook; artifact `Content/`.
2. **Item templates** — port remaining `CreateItemTemplate` calls from `Items.bb`.
3. **NPC updaters** — port missing `UpdateXXX` blocks from `NPCs.bb`.
4. **FillRoom audit** — diff BB `Case` list vs `FillRoomSystem` switch; add missing rooms.
5. **Physics** — wire `PhysicsSystem` to room `CollisionMesh` and player collider.

## Parity audit

```powershell
python agent-tools/parity_audit.py
```

Outputs JSON with line counts, FillRoom case counts, event handler counts, and NPC metrics.